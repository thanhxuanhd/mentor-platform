namespace Contract.Dtos.MentorApplications.Responses;

public class FilterMentorApplicationResponse
{
    public Guid MentorApplicationId { get; set; }
    public string? ProfilePhotoUrl { get; set; }
    public string? MentorName { get; set; }
    public string Email { get; set; } = null!;
    public string? Bio { get; set; }
    public string? Experiences { get; set; } = null!;
    public DateTime SubmittedAt { get; set; }
    public string Status { get; set; } = null!;
    public List<string> Expertises { get; set; } = new List<string>();
}
