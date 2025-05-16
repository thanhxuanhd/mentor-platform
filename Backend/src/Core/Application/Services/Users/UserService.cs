using Contract.Dtos.Users.Extensions;
using Contract.Dtos.Users.Paginations;
using Contract.Dtos.Users.Requests;
using Contract.Dtos.Users.Responses;
using Contract.Repositories;
using Contract.Shared;
using Domain.Enums;
using System.Net;

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
            return Result.Failure<GetUserResponse>($"User with id {id} not found.", HttpStatusCode.NotFound);
        }

        var userResponse = user.ToGetUserResponse();

        return Result.Success(userResponse, HttpStatusCode.OK);
    }

    public async Task<Result<PaginatedList<GetUserResponse>>> FilterUserAsync(UserFilterPagedRequest request)
    {
        var users = userRepository.GetAll();

        if (!string.IsNullOrEmpty(request.FullName))
        {
            users = users.Where(user => user.FullName.Contains(request.FullName));
        }

        if (!string.IsNullOrEmpty(request.RoleName))
        {
            users = users.Where(user => user.Role.Name.ToString().Equals(request.RoleName));
        }

        var usersResponse = users.Select(u => new GetUserResponse()
        {
            Id = u.Id,
            FullName = u.FullName,
            Email = u.Email,
            RoleId = u.RoleId,
            Status = u.Status,
            JoinedDate = u.JoinedDate,
            LastActive = u.LastActive
        });

        PaginatedList<GetUserResponse> paginatedUsers = await userRepository.ToPaginatedListAsync(usersResponse, request.PageSize, request.PageIndex);

        return Result.Success(paginatedUsers, HttpStatusCode.OK);
    }

    public async Task<Result<bool>> EditUserAsync(Guid id, EditUserRequest request)
    {
        var user = await userRepository.GetByIdAsync(id);
        if (user == null)
        {
            return Result.Failure<bool>($"User with id {id} not found.", HttpStatusCode.NotFound);
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

        user.Status = user.Status == UserStatus.Active ? UserStatus.Deactivated : UserStatus.Active;

        userRepository.Update(user);
        await userRepository.SaveChangesAsync();
        return Result.Success(true, HttpStatusCode.OK);
    }
}
