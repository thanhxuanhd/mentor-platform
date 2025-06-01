using Domain.Enums;
using FluentValidation;

namespace Contract.Dtos.CourseItems.Requests;

public record CourseItemUpdateRequest
{
    public required string Title { get; init; }
    public required string Description { get; init; }
    public required FileType MediaType { get; init; }
    public required string WebAddress { get; init; }
}

public class CourseItemUpdateRequestValidator : AbstractValidator<CourseItemUpdateRequest>
{
    public CourseItemUpdateRequestValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage("Please enter course title.")
            .MaximumLength(256)
            .WithMessage("Course title should not exceed 256 characters.");

        RuleFor(x => x.Description)
            .NotEmpty()
            .MaximumLength(256)
            .WithMessage("Description must not exceed 256 characters.");
    }
}