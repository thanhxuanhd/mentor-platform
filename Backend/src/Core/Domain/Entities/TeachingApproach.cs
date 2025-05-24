using Domain.Abstractions;

namespace Domain.Entities;

public class TeachingApproach : BaseEntity<Guid>
{
    public string? Name { get; set; }
    public virtual ICollection<UserTeachingApproach>? UserTeachingApproaches { get; set; }
}