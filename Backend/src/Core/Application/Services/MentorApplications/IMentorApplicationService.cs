using Contract.Dtos.MentorApplications.Requests;
using Contract.Dtos.MentorApplications.Responses;
using Contract.Dtos.Users.Requests;
using Contract.Shared;

namespace Application.Services.MentorApplications;

public interface IMentorApplicationService
{
    Task<Result<bool>> CreateMentorApplicationAsync(Guid userId, MentorSubmissionRequest request);
    Task<Result<PaginatedList<FilterMentorApplicationResponse>>> GetAllMentorApplicationsAsync(FilterMentorApplicationRequest request);
}
