using Domain.Abstractions;

namespace Domain.Entities;

public class MentorAvailableTimeSlot : BaseEntity<Guid>
{
    public Guid ScheduleId { get; set; }
    public DateOnly Date { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public virtual ICollection<Sessions> Sessions { get; set; } = [];
    public virtual Schedules Schedules { get; set; } = null!;
}


