using Contract.Dtos.MentorApplication.Requests;
using Contract.Dtos.MentorApplication.Responses;
using Contract.Shared;

namespace Application.Services.MentorApplications;

public interface IMentorApplicationService
{
    Task<Result<PaginatedList<FilterMentorApplicationResponse>>> GetAllMentorApplicationsAsync(FilterMentorApplicationRequest request);
    Task<Result<MentorApplicationDetailResponse>> GetMentorApplicationByIdAsync(Guid currentUserId, Guid applicationId);
    Task<Result<bool>> EditMentorApplicationAsync(Guid applicationId, UpdateMentorApplicationRequest request);
    Task<Result<RequestApplicationInfoResponse>> RequestApplicationInfoAsync(Guid adminId, Guid applicationId, RequestApplicationInfoRequest request);
    Task<Result<UpdateApplicationStatusResponse>> UpdateApplicationStatusAsync(Guid adminId, Guid applicationId, UpdateApplicationStatusRequest request);
    Task<Result<List<FilterMentorApplicationResponse>>> GetListMentorApplicationByMentorIdAsync(Guid currentUserId);
}
