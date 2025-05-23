using Domain.Abstractions;

namespace Domain.Entities;

public class UserTeachingApproach : BaseEntity<Guid>
{
    public required Guid UserId { get; set; }
    public required Guid TeachingApproachId { get; set; }
    public virtual User? User { get; set; }
    public virtual TeachingApproach? TeachingApproach { get; set; }
}