using System;
using Contract.Dtos.MentorApplication.Requests;
using Contract.Dtos.MentorApplication.Responses;
using Contract.Shared;

namespace Application.Services.Application;

public interface IMentorApplicationService
{
    Task<Result<FilterMentorApplicationResponse>> GetAllMentorApplicationsAsync(FilterMentorApplicationRequest request);
}
