using Domain.Abstractions;

namespace Domain.Entities;

public class UserExpertise : BaseEntity<Guid>
{
    public required Guid UserId { get; set; }
    public required Guid ExpertiseId { get; set; }
    public virtual User? User { get; set; }
    public virtual Expertise? Expertise { get; set; }
}