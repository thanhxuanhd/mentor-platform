using Domain.Entities;
using FluentValidation;

namespace Contract.Dtos.Users.Requests
{
    public record MentorSubmissionRequest(
        string? Education,
        string? WorkExperience,
        string? Certifications,
        string? Statement,
        List<string> DocumentURLs
        )
    {
        public void ToMentorApplication(MentorApplication application)
        {
            application.Education = Education;
            application.Certifications = Certifications;
            application.Statement = Statement;
        }
    }

    public class SubmitMentorApplicationRequestValidator : AbstractValidator<MentorSubmissionRequest>
    {
        public SubmitMentorApplicationRequestValidator()
        {

            RuleFor(x => x.Education)
                .MaximumLength(300).WithMessage("Education must be less than 300 characters.")
                .When(x => !string.IsNullOrWhiteSpace(x.Education));

            RuleFor(x => x.WorkExperience)
                .MaximumLength(300).WithMessage("Work Experience must be less than 300 characters.")
                .When(x => !string.IsNullOrWhiteSpace(x.WorkExperience));

            RuleFor(x => x.Certifications)
                .MaximumLength(300).WithMessage("Certifications must be less than 300 characters.")
                .When(x => !string.IsNullOrWhiteSpace(x.Certifications));

            RuleFor(x => x.Statement)
                .MaximumLength(300).WithMessage("Statement must be less than 300 characters.")
                .When(x => !string.IsNullOrWhiteSpace(x.Statement));

            RuleFor(x => x.DocumentURLs)
                .Must(list => list.Count <= 5).WithMessage("You can upload a maximum of 5 documents.");

        }
    }
}
