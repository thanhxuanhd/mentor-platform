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
            Status = user.Status.ToString(),
            JoinedDate = user.JoinedDate,
            LastActive = user.LastActive
        };
    }

    public static GetUserDetailResponse ToGetUserDetailResponse(this User user)
    {
        return new GetUserDetailResponse(
            RoleId: user.RoleId,
            Bio: user.Bio,
            Experiences: user.Experiences,
            FullName: user.FullName,
            Goal: user.Goal,
            IsAllowedMessage: user.IsAllowedMessage,
            IsPrivate: user.IsPrivate,
            IsReceiveNotification: user.IsReceiveNotification,
            ProfilePhotoUrl: user.ProfilePhotoUrl,
            PhoneNumber: user.PhoneNumber,
            JoinedDate: user.JoinedDate,
            PreferredSessionFrequency: user.PreferredSessionFrequency,
            PreferredCommunicationMethod: user.PreferredCommunicationMethod,
            PreferredLearningStyle: user.PreferredLearningStyle,
            PreferredSessionDuration: user.PreferredSessionDuration,
            Skills: user.Skills,
            ExpertiseIds: user.UserExpertises.Select(ue => ue.ExpertiseId).ToList(),
            AvailabilityIds: user.UserAvailabilities.Select(ua => ua.AvailabilityId).ToList(),
            CategoryIds: user.UserCategories.Select(uc => uc.CategoryId).ToList(),
            TeachingApproachIds: user.UserTeachingApproaches.Select(ut => ut.TeachingApproachId).ToList()
        );
    }
}

