using Domain.Enums;

namespace Contract.Dtos.SessionBooking.Response;

public record AvailableTimeSlotResponse
{
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public DateOnly Date { get; set; }
    public bool IsBooked { get; set; }
    public Guid MentorId { get; set; }
    public required string MentorName { get; init; }
}