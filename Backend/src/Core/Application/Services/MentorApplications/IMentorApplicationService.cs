using Contract.Dtos.MentorApplications.Requests;
using Contract.Dtos.MentorApplications.Responses;
using Contract.Dtos.Users.Requests;
using Contract.Shared;
using Microsoft.AspNetCore.Http;

namespace Application.Services.MentorApplications;

public interface IMentorApplicationService
{
    Task<Result<bool>> CreateMentorApplicationAsync(Guid userId, MentorSubmissionRequest request, HttpRequest httpRequest);
    Task<Result<PaginatedList<FilterMentorApplicationResponse>>> GetAllMentorApplicationsAsync(FilterMentorApplicationRequest request);
    Task<Result<MentorApplicationDetailResponse>> GetMentorApplicationByIdAsync(Guid currentUserId, Guid applicationId);
    Task<Result<bool>> EditMentorApplicationAsync(Guid applicationId, UpdateMentorApplicationRequest request);
    Task<Result<RequestApplicationInfoResponse>> RequestApplicationInfoAsync(Guid adminId, Guid applicationId, RequestApplicationInfoRequest request);
    Task<Result<UpdateApplicationStatusResponse>> UpdateApplicationStatusAsync(Guid adminId, Guid applicationId, UpdateApplicationStatusRequest request);
    Task<Result<List<FilterMentorApplicationResponse>>> GetListMentorApplicationByMentorIdAsync(Guid currentUserId);
}
