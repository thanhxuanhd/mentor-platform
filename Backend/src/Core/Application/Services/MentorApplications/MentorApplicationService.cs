using Application.Exceptions;
using Application.Helpers;
using Contract.Dtos.MentorApplications.Requests;
using Contract.Dtos.MentorApplications.Responses;
using Contract.Dtos.Users.Requests;
using Contract.Repositories;
using Contract.Services;
using Contract.Shared;
using Domain.Entities;
using Domain.Enums;
using System.Net;

namespace Application.Services.MentorApplications;

public class MentorApplicationService(IUserRepository userRepository, IMentorApplicationRepository mentorApplicationRepository, IEmailService emailService) : IMentorApplicationService
{
    public async Task<Result<bool>> CreateMentorApplicationAsync(Guid userId, MentorSubmissionRequest request)
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

        if (request.DocumentURLs != null && request.DocumentURLs.Any())
        {
            try
            {
                mentorApplication.ApplicationDocuments = request.DocumentURLs.Select(url => new ApplicationDocument
                {
                    MentorApplicationId = mentorApplication.Id,
                    DocumentUrl = FileHelper.VerifyFileUrl(userId.ToString(), url),
                    DocumentType = FileHelper.GetFileTypeFromUrl(url)
                }).ToList();
            }
            catch (InvalidOperationException ex)
            {
                return Result.Failure<bool>(ex.Message, HttpStatusCode.BadRequest);
            }
            catch (ArgumentException ex)
            {
                return Result.Failure<bool>(ex.Message, HttpStatusCode.BadRequest);
            }
            catch (ForbiddenAccessException ex)
            {
                return Result.Failure<bool>(ex.Message, HttpStatusCode.Forbidden);
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
