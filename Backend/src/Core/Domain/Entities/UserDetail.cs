using Domain.Abstractions;
using Domain.Enums;

namespace Domain.Entities;

public class UserDetail : BaseEntity<Guid>
{
    public Guid UserId { get; set; }
    public required string FullName { get; set; }
    public string? Bio { get; set; }
    public string? ProfilePhotoUrl { get; set; }
    public required string PhoneNumber { get; set; }
    public string? Skills { get; set; }
    public string? Experiences { get; set; }
    public TimeAvailability? Availability { get; set; }
    public CommunicationMethod? PreferredCommunicationMethod { get; set; }
    public string? Goal { get; set; }
    public ProfileCompleteStatus ProfileCompleteStatus { get; set; }

    public virtual User User { get; set; } = null!;
    public virtual ICollection<UserExpertise> UserExpertises { get; set; } = null!;
}