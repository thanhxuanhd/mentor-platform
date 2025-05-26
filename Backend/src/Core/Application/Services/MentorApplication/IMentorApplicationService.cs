using Contract.Dtos.MentorApplication.Requests;
using Contract.Dtos.MentorApplication.Responses;
using Contract.Shared;

namespace Application.Services.MentorApplication;

public interface IMentorApplicationService
{
    Task<Result<PaginatedList<FilterMentorApplicationResponse>>> GetAllMentorApplicationsAsync(FilterMentorApplicationRequest request);
    Task<Result<MentorApplicationDetailResponse>> GetMentorApplicationByIdAsync(Guid currentUserId, Guid applicationId);
}
