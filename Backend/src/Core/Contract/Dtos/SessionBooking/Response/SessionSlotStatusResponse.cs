using Domain.Enums;

namespace Contract.Dtos.SessionBooking.Response;

public record SessionSlotStatusResponse
{
    public Guid SlotId { get; init; }
    public Guid MentorId { get; init; }
    public DateTime StartTime { get; init; }
    public DateTime EndTime { get; init; }
    public SessionStatus BookingStatus { get; init; }
}