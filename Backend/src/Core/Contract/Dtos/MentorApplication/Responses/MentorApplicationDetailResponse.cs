using System;
using Domain.Enums;

namespace Contract.Dtos.MentorApplication.Responses;

public class    MentorApplicationDetailResponse
{
    public Guid MentorApplicationId { get; set; }
    public string? ProfilePhotoUrl { get; set; }
    public string? MentorName { get; set; }
    public string Email { get; set; } = null!;
    public string? Bio { get; set; }
    public string? Experiences { get; set; } = null!;
    public List<string> Expertises { get; set; } = new List<string>();
    public string ApplicationStatus { get; set; } = null!;
    public DateTime SubmittedAt { get; set; }
    public DateTime? ReviewedAt { get; set; }
    public string? Note { get; set; }
    public List<Document> Documents { get; set; } = new List<Document>();
}

public class Document
{
    public Guid DocumentId { get; set; }
    public string DocumentType { get; set; } = null!;
    public string DocumentUrl { get; set; } = null!;
}
