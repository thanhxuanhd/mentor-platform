using Domain.Abstractions;

namespace Domain.Entities;

public class MentorAvailableTimeSlot : BaseEntity<Guid>
{
    public Guid MentorId { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public Guid SessionId { get; set; }
    public virtual ICollection<Booking>? Bookings { get; set; }
    public virtual User? Mentor { get; set; }
}


