using Domain.Abstractions;

namespace Domain.Entities;

public class Conversation : BaseEntity<Guid>
{
    public string Name { get; set; } = null!;
    public DateTime CreatedAt { get; set; }

    public virtual ICollection<ConversationParticipant> Participants { get; set; } = null!;
    public virtual ICollection<Message> Messages { get; set; } = null!;
}
