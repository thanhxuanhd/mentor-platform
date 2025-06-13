using Domain.Abstractions;

namespace Domain.Entities;

public class ActivityLog : BaseEntity<Guid>
{
    public Guid? UserId { get; set; }
    public string Action { get; set; } = null!;
    public DateTime Timestamp { get; set; }
    public virtual User? User { get; set; } = null!;
}