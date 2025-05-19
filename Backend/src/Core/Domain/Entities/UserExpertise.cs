using Domain.Abstractions;

namespace Domain.Entities;

public class UserExpertise : BaseEntity<Guid>
{
    public required Guid UserDetailId { get; set; }
    public required Guid ExpertiseId { get; set; }
    public virtual UserDetail? UserDetail { get; set; }
    public virtual Expertise? Expertise { get; set; }
}