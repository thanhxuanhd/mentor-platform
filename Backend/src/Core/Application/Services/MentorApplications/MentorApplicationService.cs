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

    public async Task<Result<PaginatedList<FilterMentorApplicationResponse>>> GetAllMentorApplicationsAsync(FilterMentorApplicationRequest request)
    {
        var mentorApplications = mentorApplicationRepository.GetAllApplicationsAsync();

        if (!string.IsNullOrEmpty(request.Keyword))
        {
            mentorApplications = mentorApplications.Where(x => x.Mentor.FullName.Contains(request.Keyword, StringComparison.OrdinalIgnoreCase));
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
            Expertises = x.Mentor.UserExpertises.Select(ue => ue.Expertise.Name).ToList()
        });

        PaginatedList<FilterMentorApplicationResponse> result = await mentorApplicationRepository.ToPaginatedListAsync(
            applicationInfos,
            request.PageSize,
            request.PageIndex
        );

        return Result.Success(result, HttpStatusCode.OK);
    }
}
