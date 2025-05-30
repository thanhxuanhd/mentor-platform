
using Domain.Entities;

namespace Contract.Dtos.Timeslot.Request;

public class TimeSlotRequest
{
    public Guid MentorId { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public virtual User? Mentor { get; set; }
    public virtual ICollection<Sessions>? Bookings { get; set; }
}
