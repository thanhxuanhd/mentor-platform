using Contract.Shared;

namespace Contract.Dtos.Messages.Responses;

public class GetDetailConversationResponse
{
    public Guid ConversationId { get; set; }
    public string ConversationName { get; set; } = null!;
    public PaginatedList<GetMessageResponse> Messages { get; set; } = null!;
}