using Domain.Abstractions;

namespace Domain.Entities;

public class ConversationParticipant : BaseEntity<Guid>
{
    public Guid ConversationId { get; set; }
    public Guid UserId { get; set; }
    public DateTime JoinedAt { get; set; }
    public bool IsAdmin { get; set; }

    public virtual Conversation Conversation { get; set; } = null!;
    public virtual User User { get; set; } = null!; 
}
