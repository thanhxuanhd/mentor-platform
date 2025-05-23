using Domain.Abstractions;

namespace Domain.Entities;

public class Availability : BaseEntity<Guid>
{
    public string? Name { get; set;}
    public virtual ICollection<UserAvailability>? UserAvailabilities { get; set; }
}