using Domain.Abstractions;

namespace Domain.Entities;

public class Notification : BaseEntity<Guid>
{
    public Guid MessageId { get; set; }
    public Guid UserId { get; set; }
    public bool IsRead { get; set; }
    public DateTime CreatedAt { get; set; }

    public virtual Message Message { get; set; } = null!;
    public virtual User User { get; set; } = null!;
}
