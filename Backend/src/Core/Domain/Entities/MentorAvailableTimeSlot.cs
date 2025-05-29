using Domain.Abstractions;
using Domain.Enums;

namespace Domain.Entities;

public class MentorAvailableTimeSlot : BaseEntity<Guid>
{
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public SessionStatus Status { get; set; }
    public Guid MentorId { get; set; }
    public User Mentor { get; set; } = null!;
    public Guid ScheduleId { get; set; }
    public Schedule Schedule { get; set; } = null!;
    public List<Booking> Bookings { get; set; } = [];
}