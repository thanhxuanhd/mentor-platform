using Domain.Abstractions;

namespace Domain.Entities;

public class Schedule : BaseEntity<Guid>
{
    public Guid MentorId { get; set; }
    public DayOfWeek DayOfWeek { get; set; } 
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public int SessionDuration { get; set; } 
    public int BufferTime { get; set; } 
    public bool IsLocked { get; set; }

    public virtual User User { get; set; } = null!;
}
