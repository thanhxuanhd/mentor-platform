using System.Net;
using Contract.Dtos.Users.Extensions;
using Contract.Dtos.Users.Responses;
using Contract.Repositories;
using Contract.Shared;
using Domain.Entities;

namespace Application.Services.Users;

public class UserService(IUserRepository userRepository) : IUserService
{
    public async Task<Result<GetUserResponse>> GetUserByIdAsync(uint id)
    {
        var user = await userRepository.GetByIdAsync(id, user => user.Role);
        if (user == null)
        {
            return Result.Failure<GetUserResponse>("Null result", HttpStatusCode.BadRequest);
        }

        var userResponse = user.ToGetUserResponse();

        return Result.Success(userResponse, HttpStatusCode.OK);
    }

    // Demo pagination 
    public async Task<Result<PaginatedList<User>>> GetList()
    {
        var userList = userRepository.GetAll();
        var response = await userRepository.ToPaginatedListAsync(userList, 1 , 5);

        return Result.Success(response, HttpStatusCode.OK);
    }

}