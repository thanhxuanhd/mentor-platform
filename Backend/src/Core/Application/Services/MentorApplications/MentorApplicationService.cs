using Application.Helpers;
using Contract.Dtos.MentorApplications.Requests;
using Contract.Dtos.MentorApplications.Responses;
using Contract.Dtos.Users.Requests;
using Contract.Repositories;
using Contract.Services;
using Contract.Shared;
using Domain.Constants;
using Domain.Entities;
using Domain.Enums;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Net;

namespace Application.Services.MentorApplications;

public class MentorApplicationService(IUserRepository userRepository,
    IMentorApplicationRepository mentorApplicationRepository,
    IEmailService emailService,
    IWebHostEnvironment env,
    ILogger<MentorApplicationService> logger) : IMentorApplicationService
{
    public async Task<Result<PaginatedList<FilterMentorApplicationResponse>>> GetAllMentorApplicationsAsync(FilterMentorApplicationRequest request)
    {
        var mentorApplications = mentorApplicationRepository.GetAllApplicationsAsync();

        if (!string.IsNullOrEmpty(request.Keyword))
        {
            mentorApplications = mentorApplications.Where(x => x.Mentor.FullName.Contains(request.Keyword));
        }

        if (request.Status.HasValue && Enum.IsDefined(typeof(ApplicationStatus), request.Status.Value))
        {
            mentorApplications = mentorApplications.Where(x => x.Status == request.Status.Value);
        }

        var applicationInfos = mentorApplications.Select(x => new FilterMentorApplicationResponse
        {
            MentorApplicationId = x.Id,
            ProfilePhotoUrl = x.Mentor.ProfilePhotoUrl,
            MentorName = x.Mentor.FullName,
            Email = x.Mentor.Email,
            Bio = x.Mentor.Bio,
            Experiences = x.Mentor.Experiences,
            SubmittedAt = x.SubmittedAt,
            Status = x.Status.ToString(),
            Expertises = x.Mentor.UserExpertises.Select(ue => ue.Expertise.Name).ToList()
        }).OrderByDescending(x => x.SubmittedAt);

        PaginatedList<FilterMentorApplicationResponse> result = await mentorApplicationRepository.ToPaginatedListAsync(
            applicationInfos,
            request.PageSize,
            request.PageIndex
        );

        return Result.Success(result, HttpStatusCode.OK);
    }

    public async Task<Result<MentorApplicationDetailResponse>> GetMentorApplicationByIdAsync(Guid currentUserId, Guid applicationId)
    {
        var user = await userRepository.GetByIdAsync(currentUserId, user => user.Role);

        if (user!.Role.Name == UserRole.Learner)
        {
            return Result.Failure<MentorApplicationDetailResponse>(
                "You do not have permission to view this mentor application.", HttpStatusCode.Forbidden
            );
        }

        var applicationDetails = await mentorApplicationRepository.GetMentorApplicationByIdAsync(applicationId);

        if (applicationDetails == null)
        {
            return Result.Failure<MentorApplicationDetailResponse>(
                "Mentor application not found.", HttpStatusCode.NotFound
            );
        }

        if (user.Role.Name == UserRole.Mentor && applicationDetails.MentorId != currentUserId)
        {
            return Result.Failure<MentorApplicationDetailResponse>(
                "You do not have permission to view this mentor application.", HttpStatusCode.Forbidden
            );
        }

        var response = new MentorApplicationDetailResponse
        {
            MentorApplicationId = applicationDetails.Id,
            ProfilePhotoUrl = applicationDetails.Mentor.ProfilePhotoUrl,
            MentorName = applicationDetails.Mentor.FullName,
            Email = applicationDetails.Mentor.Email,
            Bio = applicationDetails.Mentor.Bio,
            Experiences = applicationDetails.Mentor.Experiences,
            Statement = applicationDetails.Statement,
            Certifications = applicationDetails.Certifications,
            Education = applicationDetails.Education,
            Expertises = applicationDetails.Mentor.UserExpertises.Select(ue => ue.Expertise.Name).ToList(),
            ApplicationStatus = applicationDetails.Status.ToString(),
            SubmittedAt = applicationDetails.SubmittedAt,
            ReviewedAt = applicationDetails.ReviewedAt,
            Note = applicationDetails.Note,
            ReviewBy = applicationDetails.Admin?.FullName,
            Documents = applicationDetails.ApplicationDocuments.Select(doc => new DocumentResponse
            {
                DocumentId = doc.Id,
                DocumentType = doc.DocumentType.ToString(),
                DocumentUrl = doc.DocumentUrl
            }).ToList()
        };

        return Result.Success(response, HttpStatusCode.OK);
    }

    public async Task<Result<List<FilterMentorApplicationResponse>>> GetListMentorApplicationByMentorIdAsync(Guid currentUserId)
    {
        var user = await userRepository.GetByIdAsync(currentUserId, user => user.Role);

        if (user!.Role.Name == UserRole.Learner)
        {
            return Result.Failure<List<FilterMentorApplicationResponse>>(
                "You do not have permission to view this mentor application.", HttpStatusCode.Forbidden
            );
        }

        var applicationDetails = mentorApplicationRepository
            .GetMentorApplicationByMentorIdAsync(currentUserId)
            .Select(application => new FilterMentorApplicationResponse
            {
                MentorApplicationId = application.Id,
                ProfilePhotoUrl = application.Mentor.ProfilePhotoUrl,
                MentorName = application.Mentor.FullName,
                Email = application.Mentor.Email,
                Bio = application.Mentor.Bio,
                Experiences = application.Mentor.Experiences,
                Expertises = application.Mentor.UserExpertises.Select(ue => ue.Expertise!.Name).ToList(),
                Status = application.Status.ToString(),
                SubmittedAt = application.SubmittedAt
            });

        var response = await mentorApplicationRepository.ToListAsync(applicationDetails);

        return Result.Success(response, HttpStatusCode.OK);
    }

    public async Task<Result<RequestApplicationInfoResponse>> RequestApplicationInfoAsync(Guid adminId, Guid applicationId, RequestApplicationInfoRequest request)
    {
        var application = await mentorApplicationRepository.GetMentorApplicationByIdAsync(applicationId);

        if (application == null)
        {
            return Result.Failure<RequestApplicationInfoResponse>(
                "Mentor application not found.", HttpStatusCode.NotFound
            );
        }

        if (application.Status != ApplicationStatus.Submitted)
        {
            return Result.Failure<RequestApplicationInfoResponse>(
                "You can only request additional information for submitted applications.", HttpStatusCode.Conflict
            );
        }

        application.ReviewedAt = DateTime.Now;
        application.AdminId = adminId;
        application.Status = ApplicationStatus.WaitingInfo;
        application.Note = request.Note;

        mentorApplicationRepository.Update(application);
        await mentorApplicationRepository.SaveChangesAsync();

        var subject = EmailConstants.SUBJECT_REQUEST_APPLICATION_INFO;
        var body = EmailConstants.BodyRequestApplicationInfoEmail(application.Mentor.FullName);

        var emailSent = await emailService.SendEmailAsync(
            application.Mentor.Email,
            subject,
            body
        );

        if (!emailSent)
        {
            return Result.Failure<RequestApplicationInfoResponse>(
                "Failed to send notification email.", HttpStatusCode.InternalServerError
            );
        }

        var result = new RequestApplicationInfoResponse
        {
            Message = "Request for additional information has been sent successfully.",
        };

        return Result.Success(result, HttpStatusCode.OK);
    }

    public async Task<Result<UpdateApplicationStatusResponse>> UpdateApplicationStatusAsync(Guid adminId, Guid applicationId, UpdateApplicationStatusRequest request)
    {
        var application = await mentorApplicationRepository.GetMentorApplicationByIdAsync(applicationId);

        if (application == null)
        {
            return Result.Failure<UpdateApplicationStatusResponse>(
                "Mentor application not found.", HttpStatusCode.NotFound
            );
        }

        if (application.Status == ApplicationStatus.Approved || application.Status == ApplicationStatus.Rejected)
        {
            return Result.Failure<UpdateApplicationStatusResponse>(
                "Application is already approved or rejected.", HttpStatusCode.Conflict
            );
        }

        application.Status = request.Status;
        application.Note = request.Note;
        application.ReviewedAt = DateTime.Now;
        application.AdminId = adminId;

        mentorApplicationRepository.Update(application);
        await mentorApplicationRepository.SaveChangesAsync();

        var subject = EmailConstants.SUBJECT_MENTOR_APPLICATION_DECISION;
        var body = EmailConstants.BodyMentorApplicationDecisionEmail(application.Mentor.FullName, request.Status.ToString(), request.Note);

        var emailSent = await emailService.SendEmailAsync(
            application.Mentor.Email,
            subject,
            body
        );

        if (!emailSent)
        {
            return Result.Failure<UpdateApplicationStatusResponse>(
                "Failed to send notification email.", HttpStatusCode.InternalServerError
            );
        }

        var result = new UpdateApplicationStatusResponse
        {
            Message = "Mentor application status updated successfully.",
        };

        return Result.Success(result, HttpStatusCode.OK);
    }

    public async Task<Result<bool>> EditMentorApplicationAsync(Guid applicationId, UpdateMentorApplicationRequest request, HttpRequest httpRequest)
    {
        var application = await mentorApplicationRepository.GetMentorApplicationsToUpdate(applicationId);

        if (application == null)
        {
            return Result.Failure<bool>("Mentor application not found.", HttpStatusCode.NotFound);
        }
        if (application.Status != ApplicationStatus.WaitingInfo)
        {
            return Result.Failure<bool>("You can only update applications when the status is WaitingInfo.", HttpStatusCode.BadRequest);
        }

        application.Mentor.Experiences = request.Experiences;
        application.Certifications = request.Certifications;
        application.Education = request.Education;
        application.Statement = request.Statement;



        if (request.Documents != null && request.Documents.Any())
        {
            var path = Directory.GetCurrentDirectory();
            var documentsPath = Path.Combine(path, env.WebRootPath, "documents", $"{application.MentorId}");

            if (!Directory.Exists(documentsPath))
            {
                Directory.CreateDirectory(documentsPath);
            }

            var fileNames = request.Documents.Select(f => f.FileName).ToList();
            var duplicateFiles = fileNames.GroupBy(f => f, StringComparer.OrdinalIgnoreCase)
                .Where(g => g.Count() > 1)
                .Select(g => g.Key).ToList();

            if (duplicateFiles.Any())
            {
                return Result.Failure<bool>($"Duplicate files detected in request: {string.Join(", ", duplicateFiles)}", HttpStatusCode.BadRequest);
            }

            foreach (var doc in application.ApplicationDocuments.Where(d => d.DocumentType != FileType.ExternalWebAddress).ToList())
            {
                var oldFilePath = Path.Combine(env.WebRootPath, doc.DocumentUrl.TrimStart('/'));
                if (File.Exists(oldFilePath))
                {
                    File.Delete(oldFilePath);
                }
            }
            application.ApplicationDocuments.Clear();

            foreach (var file in request.Documents)
            {
                if (file.Length > 0)
                {
                    string fileName = $"{file.FileName}";
                    var filePath = Path.Combine(documentsPath, fileName);

                    var baseUrl = $"{httpRequest?.Scheme}://{httpRequest?.Host}";
                    var fileUrl = $"{baseUrl}/documents/{application.MentorId}/{fileName}";

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    application.ApplicationDocuments.Add(new ApplicationDocument
                    {
                        MentorApplicationId = application.Id,
                        DocumentType = FileHelper.GetFileTypeFromUrl(fileUrl),
                        DocumentUrl = fileUrl
                    });
                }
            }
        }
        application.Status = ApplicationStatus.Submitted;
        mentorApplicationRepository.Update(application);
        await mentorApplicationRepository.SaveChangesAsync();

        var subject = EmailConstants.SUBJECT_UPDATE_APPLICATION;
        var body = EmailConstants.BodyUpdatedNotificationApplication(application.Admin.FullName, application.Mentor.FullName);

        var emailSent = await emailService.SendEmailAsync(application.Admin.Email, subject, body);

        if (!emailSent)
            return Result.Failure<bool>("Failed to send notification email.", HttpStatusCode.InternalServerError);

        return Result.Success(true, HttpStatusCode.OK);
    }

    public async Task<Result<bool>> CreateMentorApplicationAsync(Guid userId, MentorSubmissionRequest submission, HttpRequest httpRequest)
    {
        var user = await userRepository.GetByIdAsync(userId, u => u.MentorApplications);
        if (user == null)
        {
            return Result.Failure<bool>("User not found", HttpStatusCode.NotFound);
        }

        if (user.MentorApplications != null && user.MentorApplications.Any(x => x.Status != ApplicationStatus.Rejected))
        {
            return Result.Failure<bool>("User has an active or pending mentor application.", HttpStatusCode.BadRequest);
        }

        user.Experiences = submission.WorkExperience;
        userRepository.Update(user);

        var mentorApplication = new MentorApplication
        {
            MentorId = userId,
            SubmittedAt = DateTime.UtcNow,
        };
        submission.ToMentorApplication(mentorApplication);

        if (submission.Documents != null && submission.Documents.Any())
        {
            mentorApplication.ApplicationDocuments = new List<ApplicationDocument>();
            var path = Directory.GetCurrentDirectory();
            logger.LogInformation($"RootPath: {env.WebRootPath}");

            var documentsPath = Path.Combine(path, env.WebRootPath, "documents", $"{userId}");
            if (!Directory.Exists(documentsPath))
            {
                Directory.CreateDirectory(documentsPath);
            }

            foreach (var file in submission.Documents)
            {
                if (file == null || file.Length == 0)
                {
                    return Result.Failure<bool>("File not selected", HttpStatusCode.BadRequest);
                }

                var fileContentType = file.ContentType;
                if (!FileConstants.DOCUMENT_CONTENT_TYPES.Contains(fileContentType))
                {
                    return Result.Failure<bool>("File content type is not allowed.", HttpStatusCode.BadRequest);
                }

                if (file.Length > FileConstants.MAX_FILE_SIZE)
                {
                    return Result.Failure<bool>("File size must not exceed 1MB.", HttpStatusCode.BadRequest);
                }

                var filePath = Path.Combine(documentsPath, file.FileName);

                try
                {
                    using var stream = new FileStream(filePath, FileMode.Create);
                    await file.CopyToAsync(stream);

                    var baseUrl = $"{httpRequest?.Scheme}://{httpRequest?.Host}";
                    var fileUrl = $"{baseUrl}/documents/{userId}/{file.FileName}";

                    mentorApplication.ApplicationDocuments.Add(new ApplicationDocument
                    {
                        MentorApplicationId = mentorApplication.Id,
                        DocumentUrl = fileUrl,
                        DocumentType = FileHelper.GetFileTypeFromUrl(fileUrl)
                    });
                }
                catch (Exception ex)
                {
                    return Result.Failure<bool>($"Failed to save file: {ex.Message}", HttpStatusCode.InternalServerError);
                }
            }
        }

        await mentorApplicationRepository.AddAsync(mentorApplication);
        await mentorApplicationRepository.SaveChangesAsync();

        return Result.Success(true, HttpStatusCode.OK);
    }
}
