namespace Contract.Dtos.Messages.Responses;

public class GetMinimalConversationResponse
{
    public Guid ConversationId { get; set; }
    public string ConversationName { get; set; } = null!;
    public GetMessageResponse? LastMessage { get; set; } = null!;
    public List<ConversationParticipantResponse> Participants { get; set; } = null!;
}