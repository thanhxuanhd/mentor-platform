using Domain.Abstractions;
using Domain.Enums;

namespace Domain.Entities;

public class Sessions : BaseEntity<Guid>
{
    public Guid TimeSlotId { get; set; }
    public Guid LearnerId { get; set; }
    public SessionStatus Status { get; set; }
    public MentorAvailableTimeSlot TimeSlot { get; set; } = null!;
    public User Learner { get; set; } = null!;
}
