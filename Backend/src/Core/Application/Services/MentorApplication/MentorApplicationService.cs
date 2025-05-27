using System.Net;
using Contract.Dtos.MentorApplication.Requests;
using Contract.Dtos.MentorApplication.Responses;
using Contract.Repositories;
using Contract.Shared;
using Domain.Constants;
using Domain.Entities;
using Domain.Enums;

namespace Application.Services.MentorApplication;

public class MentorApplicationService(IUserRepository userRepository, IMentorApplicationRepository mentorApplicationRepository) : IMentorApplicationService
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
        });

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
            }).ToList(),
        };

        return Result.Success(response, HttpStatusCode.OK);
    }

    public async Task<Result<bool>> EditMentorApplicationAsync(Guid applicationId, UpdateMentorApplicationRequest request)
    {
        var application = await mentorApplicationRepository.GetByIdAsync(applicationId);
        if (application == null)
        {
            return Result.Failure<bool>("Mentor application not found.", HttpStatusCode.NotFound);
        }
        if (application.Status != ApplicationStatus.WaitingInfo)
        {
            return Result.Failure<bool>("You can only update applications when the status is WaitingInfo.", HttpStatusCode.BadRequest);
        }

        application.ApplicationDocuments = [.. request.Documents!.Select(doc => new ApplicationDocument
        {
            DocumentType = doc.DocumentType,
            DocumentUrl = doc.DocumentUrl
        })];
        application.Status = ApplicationStatus.Submitted;
        mentorApplicationRepository.Update(application);
        await mentorApplicationRepository.SaveChangesAsync();

        var subject = EmailConstants.SUBJECT_UPDATE_APPLICATION;
        var body = EmailConstants.BodyUpdatedNotificationApplication(application.Id);

        return Result.Success(true, HttpStatusCode.OK);
    }
}