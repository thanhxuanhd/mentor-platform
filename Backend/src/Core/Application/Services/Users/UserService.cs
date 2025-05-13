using Contract.Dtos.Users.Extensions;
using Contract.Dtos.Users.Responses;
using Contract.Repositories;
using Contract.Shared;

namespace Application.Services.Users;

public class UserService(IUserRepository userRepository) : IUserService
{
    public async Task<Result<GetUserResponse>> GetUserByIdAsync(uint id)
    {
        var user = await userRepository.GetByIdAsync(id, user => user.Role);
        if (user == null)
        {
            return Result.Failure<GetUserResponse>("Null result");
        }

        var userResponse = user.ToGetUserResponse();

        return Result.Success(userResponse);
    }
}