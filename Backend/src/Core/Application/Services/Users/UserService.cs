using Contract.Dtos.Users.Extensions;
using Contract.Dtos.Users.Responses;
using Contract.Repositories;
using Contract.Shared;
using System.Net;
using Domain.Entities;

namespace Application.Services.Users;

public class UserService(IUserRepository userRepository) : IUserService
{
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

    public async Task<PaginatedList<User>> GetList()
    {
        var user = userRepository.GetAll();
        return await userRepository.ToPaginatedListAsync<User>(user, 0, 0);
    }
}