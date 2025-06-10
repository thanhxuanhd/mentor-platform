using Contract.Shared;

namespace Contract.Dtos.Messages.Responses;

public class GetFilterConversationResponse
{
    public Guid Id { get; set; }
    public string PhotoUrl { get; set; } = null!;
    public string Name { get; set; } = null!;
    public Guid? ConversationId { get; set; }
    public bool IsGroup { get; set; }
}