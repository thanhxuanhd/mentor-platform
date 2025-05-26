using Domain.Abstractions;

namespace Domain.Entities;

public class UserCategory : BaseEntity<Guid>
{
    public required Guid UserId { get; set; }
    public required Guid CategoryId { get; set; }
    public virtual User? User { get; set; }
    public virtual Category? Category { get; set; }
}