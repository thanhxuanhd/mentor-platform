using Domain.Enums;
using FluentValidation;

namespace Contract.Dtos.MentorApplication.Requests;

public class UpdateApplicationStatusRequest
{
    public ApplicationStatus Status { get; set; }
    public string? Note { get; set; }
}

public class UpdateApplicationStatusRequestValidator : AbstractValidator<UpdateApplicationStatusRequest>
{
    public UpdateApplicationStatusRequestValidator()
    {
        RuleFor(x => x.Status)
            .Must(status => status == ApplicationStatus.Approved || status == ApplicationStatus.Rejected)
            .WithMessage("Application status must be either Approved or Rejected.");

        RuleFor(x => x.Note)
            .MaximumLength(300)
            .WithMessage("Note must not exceed 300 characters.");
    }
}
