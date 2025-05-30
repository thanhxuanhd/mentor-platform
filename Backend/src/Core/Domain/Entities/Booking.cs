using Domain.Abstractions;
using Domain.Enums;

namespace Domain.Entities;

public class Sessions : BaseEntity<Guid>
{
    public Guid TimeSlotId { get; set; }
    public Guid LearnerId { get; set; }
    public SessionStatus Status { get; set; }
    public virtual MentorAvailableTimeSlot? TimeSlot { get; set; }
    public virtual User? Learner { get; set; }
}
