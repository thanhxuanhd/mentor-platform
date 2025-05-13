using Contract.Dtos.Users.Responses;
using Contract.Shared;

namespace Application.Services.Users;

public interface IUserService
{
    Task<Result<GetUserResponse>> GetUserByIdAsync(uint id);
}