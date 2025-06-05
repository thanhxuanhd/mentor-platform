using Domain.Enums;

namespace Contract.Dtos.SessionBooking.Response;

public record SessionSlotStatusResponse
{
    public Guid SlotId { get; init; }
    public Guid MentorId { get; init; }
    public DateOnly Day { get; init; }
    public TimeOnly StartTime { get; init; }
    public TimeOnly EndTime { get; init; }
    public SessionStatus BookingStatus { get; init; }
}