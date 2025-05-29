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
}
