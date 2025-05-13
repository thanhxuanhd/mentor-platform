using Contract.Dtos.Users.Responses;
using Domain.Entities;

namespace Contract.Dtos.Users.Extensions;

public static class UserResponseExtensions
{
    public static GetUserResponse ToGetUserResponse(this User user)
    {
        return new GetUserResponse
        {
            Username = user.Username,
            Email = user.Email,
            Role = user.Role.Name
        };
    }
}