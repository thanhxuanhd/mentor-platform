using Contract.Dtos.Users.Responses;
using Domain.Entities;

namespace Contract.Dtos.Users.Extensions;

public static class UserResponseExtensions
{
    public static GetUserResponse ToGetUserResponse(this User user)
    {
        return new GetUserResponse
        {
            Id = user.Id,
            FullName = user.FullName,
            Email = user.Email,
            Role = user.Role.Name.ToString(),
            Status = user.Status,
            JoinedDate = user.JoinedDate,
            LastActive = user.LastActive
        };
    }
}