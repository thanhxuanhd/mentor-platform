using Domain.Enums;

namespace Contract.Dtos.SessionBooking.Response;

public record SessionSlotStatusResponse
{
    public required Guid SlotId { get; init; }
    public required Guid MentorId { get; init; }
    public required DateOnly Day { get; init; }
    public required TimeOnly StartTime { get; init; }
    public required TimeOnly EndTime { get; init; }
    public required SessionStatus BookingStatus { get; init; }
}