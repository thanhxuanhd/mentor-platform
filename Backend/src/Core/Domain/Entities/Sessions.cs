using Domain.Abstractions;
using Domain.Enums;

namespace Domain.Entities;

public class Sessions : BaseEntity<Guid>
{
    public Guid TimeSlotId { get; set; }
    public Guid LearnerId { get; set; }
    public SessionStatus Status { get; set; }
    public SessionType Type { get; set; }
    public virtual MentorAvailableTimeSlot TimeSlot { get; set; } = null!;
    public virtual User Learner { get; set; } = null!;
}
