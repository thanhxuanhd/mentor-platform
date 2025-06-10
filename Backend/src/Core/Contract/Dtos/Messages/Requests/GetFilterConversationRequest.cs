namespace Contract.Dtos.Messages.Requests;

public record GetFilterConversationRequest(string Keyword, int PageIndex);