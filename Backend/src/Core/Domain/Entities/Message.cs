using Domain.Abstractions;

namespace Domain.Entities;

public class Message : BaseEntity<Guid>
{
    public Guid ConversationId { get; set; }
    public Guid SenderId { get; set; }
    public string Content { get; set; } = null!;
    public DateTime SentAt { get; set; }

    public virtual Conversation Conversation { get; set; } = null!;
    public virtual User Sender { get; set; } = null!;
    public virtual ICollection<Notification> Notifications { get; set; } = [];
}
