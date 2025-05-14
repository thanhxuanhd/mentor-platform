using Contract.Dtos.Users.Extensions;
using Contract.Dtos.Users.Responses;
using Contract.Repositories;
using Contract.Shared;
using System.Net;
using Contract.Dtos.Users.Paginations;
using Domain.Enums;
using Contract.Dtos.Users.Requests;

namespace Application.Services.Users;

public class UserService(IUserRepository userRepository) : IUserService
{
    public async Task<Result<GetUserResponse>> GetUserByIdAsync(Guid id)
    {
        var user = await userRepository.GetByIdAsync(id, user => user.Role);
        if (user == null)
        {
            return Result.Failure<GetUserResponse>("Null result", HttpStatusCode.NotFound);
        }

        var userResponse = user.ToGetUserResponse();

        return Result.Success(userResponse, HttpStatusCode.OK);
    }

    public async Task<Result<PaginatedList<GetUserResponse>>> FilterUserAsync(UserFilterPagedRequest request)
    {
        var users = await userRepository.FilterUser(request);
       
        var userResponses = users.Items.Select(user => user.ToGetUserResponse()).ToList();

        var paginatedResponse = new PaginatedList<GetUserResponse>(userResponses, users.TotalCount, users.PageIndex, users.PageSize);

        return Result.Success(paginatedResponse, HttpStatusCode.OK);
    }

    public async Task<Result<bool>> EditUserAsync(Guid id, EditUserRequest request)
    {
        var user = await userRepository.GetByIdAsync(id);
        if (user == null)
        {
            return Result.Failure<bool>("Null result", HttpStatusCode.NotFound);
        }
        user.FullName = request.FullName;
        user.Email = request.Email;
        user.RoleId = request.RoleId;

        userRepository.Update(user);
        await userRepository.SaveChangesAsync();

        return Result.Success(true, HttpStatusCode.OK);
    }

    public async Task<Result<bool>> ChangeUserStatusAsync(Guid userId)
    {
        var user = await userRepository.GetByIdAsync(userId);

        if (user == null)
        {
            return Result.Failure<bool>($"User with id {userId} not found.", HttpStatusCode.NotFound);
        }

        if (user.Status.Equals(UserStatus.Active))
        {
            user.Status = UserStatus.Deactivated;
        }
        else
        {
            user.Status = UserStatus.Active;
        }

        userRepository.Update(user);
        await userRepository.SaveChangesAsync();
        return Result.Success(true, HttpStatusCode.OK);
    }
}