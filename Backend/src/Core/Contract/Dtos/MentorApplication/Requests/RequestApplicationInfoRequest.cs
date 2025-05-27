using FluentValidation;

namespace Contract.Dtos.MentorApplication.Requests;

public class RequestApplicationInfoRequest
{
    public string? Note { get; set; }
}

public class RequestApplicationInfoRequestValidator : AbstractValidator<RequestApplicationInfoRequest>
{
    public RequestApplicationInfoRequestValidator()
    {
        RuleFor(x => x.Note)
            .MaximumLength(300)
            .WithMessage("Note must not exceed 300 characters.");
    }
}