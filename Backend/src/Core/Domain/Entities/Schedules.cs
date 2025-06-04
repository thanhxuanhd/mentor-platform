using Domain.Abstractions;

namespace Domain.Entities;

public class Schedules : BaseEntity<Guid>
{
    public Guid MentorId { get; set; }
    public DateOnly WeekStartDate { get; set; }
    public DateOnly WeekEndDate { get; set; }
    public TimeOnly StartHour { get; set; }
    public TimeOnly EndHour { get; set; }
    public int SessionDuration { get; set; } 
    public int BufferTime { get; set; } 

    public virtual ICollection<MentorAvailableTimeSlot>? AvailableTimeSlots { get; set; } 
    public virtual User Mentor { get; set; } = null!;
}
