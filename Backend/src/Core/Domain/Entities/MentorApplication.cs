using Domain.Abstractions;
using Domain.Enums;

namespace Domain.Entities;

public class MentorApplication : BaseEntity<Guid>
{
    public Guid MentorId { get; set; }
    public Guid? AdminId { get; set; }
    public ApplicationStatus Status { get; set; }
    public DateTime SubmittedAt { get; set; }
    public DateTime? ReviewedAt { get; set; }
    public string? Education { get; set; }
    public string? Certifications { get; set; }
    public string? Statement { get; set; }
    public string? Note { get; set; }
    public User Mentor { get; set; } = null!;
    public User? Admin { get; set; }
    public ICollection<ApplicationDocument> ApplicationDocuments { get; set; } = [];
}