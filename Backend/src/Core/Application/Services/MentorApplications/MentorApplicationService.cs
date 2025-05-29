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
using Microsoft.Extensions.Logging;
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

    private static FileType GetFileTypeFromUrl(string url)
    {
        if (string.IsNullOrWhiteSpace(url))
        {
            throw new ArgumentException("URL cannot be null or empty", nameof(url));
        }

        if (!Uri.TryCreate(url, UriKind.RelativeOrAbsolute, out var uri))
        {
            throw new InvalidOperationException("Invalid document URL format.");
        }

        var path = uri.IsAbsoluteUri ? uri.LocalPath : url;
        var fileName = Path.GetFileName(path);
        var fileTypeString = fileName.Split("-")[0];

        return fileTypeString.ToLower() switch
        {
            "pdf" => FileType.Pdf,
            "video" => FileType.Video,
            "audio" => FileType.Audio,
            "image" => FileType.Image,
            _ => throw new InvalidOperationException($"Unknown file type: {fileTypeString}")
        };
    }

    public async Task<Result<bool>> CreateMentorApplicationAsync(Guid userId, MentorSubmissionRequest request, HttpRequest httpRequest)
    {
        var user = await userRepository.GetByIdAsync(userId);
        if (user == null)
        {
            return Result.Failure<bool>("User not found", HttpStatusCode.NotFound);
        }

        user.Experiences = request.WorkExperience;
        userRepository.Update(user);

        var mentorApplication = new MentorApplication
        {
            MentorId = userId,
            SubmittedAt = DateTime.UtcNow,
        };
        request.ToMentorApplication(mentorApplication);

        if (request.Documents != null && request.Documents.Any())
        {
            mentorApplication.ApplicationDocuments = new List<ApplicationDocument>();
            var path = Directory.GetCurrentDirectory();
            logger.LogInformation($"RootPath: {env.WebRootPath}");

            var documentsPath = Path.Combine(path, env.WebRootPath, "documents", $"{userId}");
            if (!Directory.Exists(documentsPath))
            {
                Directory.CreateDirectory(documentsPath);
            }

            foreach (var file in request.Documents)
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

                long epoch = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                string fileName = $"{epoch}_{file.FileName}";
                var filePath = Path.Combine(documentsPath, fileName);

                try
                {
                    using var stream = new FileStream(filePath, FileMode.Create);
                    await file.CopyToAsync(stream);

                    var baseUrl = $"{httpRequest?.Scheme}://{httpRequest?.Host}";
                    var fileUrl = $"{baseUrl}/documents/{userId}/{fileName}";

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
