using Domain.Enums;

namespace Contract.Dtos.SessionBooking.Response;

public record TimeSlotByMentorAndDateResponse
{
    public required Guid Id { get; init; }
    public required TimeOnly StartTime { get; init; }
    public required TimeOnly EndTime { get; init; }
    public required DateOnly Date { get; init; }
    public required Guid MentorId { get; init; }
    public required string MentorName { get; init; }
    public required bool IsBooked { get; init; }
    public required SessionStatus? LearnerCurrentBookingStatus { get; init; }
}