namespace Contract.Dtos.Messages.Responses;

public class GetDetailMessageResponse
{
    public Guid MessageId { get; set; }
    public string Content { get; set; } = null!;
    public DateTime SentAt { get; set; }
    public Guid SenderId { get; set; }
    public string SenderName { get; set; } = null!;
    public string SenderProfilePhotoUrl { get; set; } = null!;
}