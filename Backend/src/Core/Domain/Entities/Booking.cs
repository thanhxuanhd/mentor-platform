using Domain.Abstractions;
using Domain.Enums;

namespace Domain.Entities;

public class Booking : BaseEntity<Guid>
{
    public SessionType SessionType { get; init; }
    public BookingStatus Status { get; set; }
    public DateTime BookedDateTime { get; set; }
    public Guid TimeSlotId { get; set; }
    public MentorAvailableTimeSlot TimeSlot { get; set; } = null!;
    public Guid LearnerId { get; set; }
}