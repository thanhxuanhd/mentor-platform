using Domain.Enums;

namespace Contract.Dtos.Users.Responses;

public record GetUserDetailResponse(
    string FullName,
    int RoleId,
    string? Bio,
    string? ProfilePhotoUrl,
    string? PhoneNumber,
    string? Skills,
    string? Experiences,
    DateOnly? JoinedDate,
    CommunicationMethod? PreferredCommunicationMethod,
    string? Goal,
    SessionFrequency PreferredSessionFrequency,
    int PreferredSessionDuration,
    LearningStyle PreferredLearningStyle,
    bool IsPrivate,
    bool IsAllowedMessage,
    bool IsReceiveNotification,
    List<Guid>? AvailabilityIds,
    List<Guid>? ExpertiseIds,
    List<Guid>? TeachingApproachIds,
    List<Guid>? CategoryIds
);