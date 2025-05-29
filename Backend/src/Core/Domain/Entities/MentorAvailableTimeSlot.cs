using Domain.Abstractions;
using System.Runtime.CompilerServices;

namespace Domain.Entities;

public class MentorAvailableTimeSlot : BaseEntity<Guid>
{
    public Guid MentorId { get; set; }
    public Guid SessionId { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public virtual User Mentor { get; set; }
    public virtual ICollection<Booking>? Bookings { get; set; }
}


