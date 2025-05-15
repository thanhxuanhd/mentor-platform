using Contract.Dtos.Users.Extensions;
using Contract.Dtos.Users.Responses;
using Contract.Repositories;
using Contract.Shared;
using System.Net;
using Domain.Entities;

namespace Application.Services.Users;

public class UserService(IUserRepository userRepository) : IUserService
{
    public async Task<Result<GetUserResponse>> GetUserByEmailAsync(string email)
    {
        var user = await userRepository.GetByEmailAsync(email, user => user.Role);
        if (user == null)
        {
            return Result.Failure<GetUserResponse>("User not found", HttpStatusCode.NotFound);
        }

        var userResponse = user.ToGetUserResponse();

        return Result.Success(userResponse, HttpStatusCode.OK);
    }


    public async Task<Result<GetUserResponse>> GetUserByIdAsync(Guid id)
    {
        var user = await userRepository.GetByIdAsync(id, user => user.Role);
        if (user == null)
        {
            return Result.Failure<GetUserResponse>("Null result", HttpStatusCode.BadRequest);
        }

        var userResponse = user.ToGetUserResponse();

        return Result.Success(userResponse, HttpStatusCode.OK);
    }
}