using System;
using Contract.Dtos.MentorApplication.Requests;
using Contract.Dtos.MentorApplication.Responses;
using Contract.Repositories;
using Contract.Shared;

namespace Application.Services.Application;

public class ApplicationService(IUserRepository userRepository) : IMentorApplicationService
{
    public Task<Result<FilterMentorApplicationResponse>> GetAllMentorApplicationsAsync(FilterMentorApplicationRequest request)
    {
        // Implementation of the method to filter mentor applications
        throw new NotImplementedException();
    }
}
