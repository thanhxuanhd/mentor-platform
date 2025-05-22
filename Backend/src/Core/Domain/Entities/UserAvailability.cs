using Domain.Abstractions;

namespace Domain.Entities;

public class UserAvailability : BaseEntity<Guid>
{
    public required Guid UserId { get; set; }
    public required Guid AvailabilityId { get; set; }
    public virtual User? User { get; set; }
    public virtual Availability? Availability { get; set; }
}