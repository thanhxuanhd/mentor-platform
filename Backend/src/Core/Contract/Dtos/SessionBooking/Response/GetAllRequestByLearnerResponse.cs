using Domain.Enums;

namespace Contract.Dtos.SessionBooking.Response;

public record GetAllRequestByLearnerResponse
{
    public required Guid SessionId { get; init; }
    public required Guid SlotId { get; init; }
    public required string MentorName { get; init; }
    public required List<string>? Expirtise { get; init; } = [];
    public required string? MentorAvatarUrl { get; init; }
    public required SessionType SessionType { get; init; }
    public required DateOnly Day { get; init; }
    public required TimeOnly StartTime { get; init; }
    public required TimeOnly EndTime { get; init; }
    public required SessionStatus BookingStatus { get; init; }
}
