namespace Contract.Dtos.Messages.Requests;

public record AddMessageRequest(Guid? ConversationId, Guid? RecipientId, string Content);