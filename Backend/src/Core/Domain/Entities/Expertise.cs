using Domain.Abstractions;

namespace Domain.Entities;

public class Expertise : BaseEntity<Guid>
{
    public required string Name { get; set; }
    public virtual ICollection<UserExpertise> UserExpertises { get; set; } = null!;
}