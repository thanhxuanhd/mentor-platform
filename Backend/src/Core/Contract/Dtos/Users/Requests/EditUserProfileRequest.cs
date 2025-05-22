using Domain.Entities;
using Domain.Enums;
using FluentValidation;

namespace Contract.Dtos.Users.Requests;

public record EditUserProfileRequest(
    string FullName,
    int RoleId,
    string? Bio,
    string? ProfilePhotoUrl,
    string? PhoneNumber,
    string? Skills,
    string? Experiences,
    CommunicationMethod? PreferredCommunicationMethod,
    string? Goal,
    SessionFrequency PreferredSessionFrequency,
    int PreferredSessionDuration,
    LearningStyle PreferredLearningStyle,
    bool IsPrivate,
    bool IsAllowedMessage,
    bool IsReceiveNotification,
    List<Guid>? AvailabilityIds,
    List<Guid>? ExpertiseIds,
    List<Guid>? TeachingApproachIds,
    List<Guid>? CategoryIds
)
{
    public void ToUser(User user)
    {
        user.FullName = FullName;
        user.RoleId = RoleId;
        user.Bio = Bio;
        user.ProfilePhotoUrl = ProfilePhotoUrl;
        user.PhoneNumber = PhoneNumber;
        user.Skills = Skills;
        user.Experiences = Experiences;
        user.PreferredCommunicationMethod = PreferredCommunicationMethod;
        user.Goal = Goal;
        user.PreferredSessionFrequency = PreferredSessionFrequency;
        user.PreferredSessionDuration = PreferredSessionDuration;
        user.PreferredLearningStyle = PreferredLearningStyle;
        user.IsPrivate = IsPrivate;
        user.IsAllowedMessage = IsAllowedMessage;
        user.IsReceiveNotification = IsReceiveNotification;
    }
}

public class EditUserDetailRequestValidator : AbstractValidator<EditUserProfileRequest>
{
    private static readonly List<int> _allowedDurations = new() { 30, 45, 60, 90, 120 };
    public EditUserDetailRequestValidator()
    {
        RuleFor(x => x.FullName)
            .NotEmpty().WithMessage("Full name is required")
            .MaximumLength(100).WithMessage("Full name must not exceed 100 characters");

        RuleFor(x => x.RoleId)
            .GreaterThan(0).WithMessage("Role ID is not valid");

        RuleFor(x => x.Bio)
            .MaximumLength(300).WithMessage("Bio must not exceed 300 characters")
            .When(x => x.Bio != null);

        RuleFor(x => x.ProfilePhotoUrl)
            .MaximumLength(200).WithMessage("Profile photo URL must not exceed 200 characters")
            .Must(BeAValidUrl).WithMessage("Invalid URL format for profile photo")
            .When(x => x.ProfilePhotoUrl != null);

        RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithMessage("Phone number is required")
            .MaximumLength(10).WithMessage("Phone number must not exceed 10 characters")
            .Matches(@"^\d+$").WithMessage("Phone number must contain only numbers");

        RuleFor(x => x.Skills)
            .MaximumLength(200).WithMessage("Skills must not exceed 200 characters")
            .When(x => x.Skills != null);

        RuleFor(x => x.Experiences)
            .MaximumLength(200).WithMessage("Experiences must not exceed 200 characters")
            .When(x => x.Experiences != null);

        RuleFor(x => x.Goal)
            .MaximumLength(200).WithMessage("Goal must not exceed 200 characters")
            .When(x => x.Goal != null);

        RuleFor(x => x.PreferredCommunicationMethod)
            .IsInEnum().WithMessage("Invalid communication method selected.")
            .When(x => x.PreferredCommunicationMethod != null);

        RuleFor(x => x.PreferredSessionFrequency)
            .IsInEnum().WithMessage("A valid session frequency must be selected.");

        RuleFor(x => x.PreferredSessionDuration)
            .Must(duration => _allowedDurations.Contains(duration))
            .WithMessage($"Session duration must be one of the following: {string.Join(", ", _allowedDurations)} minutes.");

        RuleFor(x => x.PreferredLearningStyle)
            .IsInEnum().WithMessage("A valid learning style must be selected.");

        RuleFor(x => x.AvailabilityIds)
            .Must(ids => ids!.All(id => id != Guid.Empty))
            .WithMessage("All Availability IDs must be non-empty Guids")
            .When(x => x.AvailabilityIds is { Count: > 0 });

        RuleFor(x => x.ExpertiseIds)
            .Must(ids => ids!.All(id => id != Guid.Empty))
            .WithMessage("All Expertise IDs must be non-empty Guids")
            .When(x => x.ExpertiseIds is { Count: > 0 });

        RuleFor(x => x.TeachingApproachIds)
            .Must(ids => ids!.All(id => id != Guid.Empty))
            .WithMessage("All Teaching Approach IDs must be non-empty Guids")
            .When(x => x.TeachingApproachIds is { Count: > 0 });

        RuleFor(x => x.CategoryIds)
            .Must(ids => ids!.All(id => id != Guid.Empty))
            .WithMessage("All Category IDs must be non-empty Guids")
            .When(x => x.CategoryIds is { Count: > 0 });
    }

    private bool BeAValidUrl(string? url)
    {
        if (string.IsNullOrEmpty(url))
            return true;
        return Uri.TryCreate(url, UriKind.Absolute, out var uriResult) &&
               (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
    }
}