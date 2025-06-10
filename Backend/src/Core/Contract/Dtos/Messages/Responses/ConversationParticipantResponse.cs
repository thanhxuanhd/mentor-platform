namespace Contract.Dtos.Messages.Responses;

public class ConversationParticipantResponse
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = null!;
    public string ProfilePhotoUrl { get; set; } = null!;
    public string Role { get; set; } = null!;
}