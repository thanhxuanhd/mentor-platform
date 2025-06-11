namespace Contract.Dtos.Messages.Requests;

public record AddMessageRequest(Guid? ConversationId, Guid? RecipientId, string Content);

public record SendMessageRequest(string ConversationId, string RecipientId, string Content);