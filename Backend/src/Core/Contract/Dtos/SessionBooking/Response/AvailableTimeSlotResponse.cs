using Domain.Enums;

namespace Contract.Dtos.SessionBooking.Response;

public record AvailableTimeSlotResponse
{
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public SessionStatus Status { get; set; }
    public Guid MentorId { get; set; }
    public required string MentorName { get; init; }
}