using Domain.Enums;
using FluentValidation;

namespace Contract.Dtos.MentorApplication.Requests;

public class FilterMentorApplicationRequest
{
    public string? Keyword { get; set; }
    public ApplicationStatus? Status { get; set; }
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 5;
}

public class FilterMentorApplicationRequestValidator : AbstractValidator<FilterMentorApplicationRequest>
{
    public FilterMentorApplicationRequestValidator()
    {
        RuleFor(x => x.Keyword)
            .MaximumLength(100)
            .WithMessage("Keyword must not exceed 100 characters.");

        RuleFor(x => x.PageIndex)
            .GreaterThanOrEqualTo(1)
            .WithMessage("Page index must be greater than or equal to 1.");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100)
            .WithMessage("Page size must be between 1 and 100.");
    }
}