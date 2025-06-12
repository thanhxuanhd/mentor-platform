using Domain.Abstractions;
using Domain.Enums;

namespace Domain.Entities;

public class User : BaseEntity<Guid>
{
    public string FullName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string? PasswordHash { get; set; }
    public UserStatus Status { get; set; }
    public DateOnly JoinedDate { get; set; }
    public DateOnly LastActive { get; set; }
    public int RoleId { get; set; }
    public string? Bio { get; set; }
    public string? ProfilePhotoUrl { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Skills { get; set; }
    public string? Experiences { get; set; }
    public CommunicationMethod? PreferredCommunicationMethod { get; set; }
    public string? Goal { get; set; }
    public SessionFrequency PreferredSessionFrequency { get; set; }
    public int PreferredSessionDuration { get; set; }
    public LearningStyle PreferredLearningStyle { get; set; }
    public bool IsPrivate { get; set; }
    public bool IsAllowedMessage { get; set; }
    public bool IsReceiveNotification { get; set; }
    public string Timezone { get; set; } = "Asia/Bangkok";
    public virtual ICollection<MentorApplication> MentorApplications { get; set; } = [];
    public virtual ICollection<MentorApplication> ReviewedMentorApplications { get; set; } = [];
    public virtual ICollection<UserTeachingApproach> UserTeachingApproaches { get; set; } = [];
    public virtual ICollection<UserCategory> UserCategories { get; set; } = [];
    public virtual ICollection<UserAvailability> UserAvailabilities { get; set; } = [];
    public virtual ICollection<UserExpertise> UserExpertises { get; set; } = [];
    public virtual Role Role { get; set; } = null!;
    public virtual ICollection<Course>? Courses { get; set; }
    public virtual ICollection<Schedules>? Schedules { get; set; }
    public virtual ICollection<Sessions>? Sessions { get; set; }
    public virtual ICollection<ActivityLog> ActivityLogs { get; set; } = null!;
}