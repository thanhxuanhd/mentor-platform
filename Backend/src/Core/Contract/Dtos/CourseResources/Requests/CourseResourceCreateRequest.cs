using Domain.Enums;
using FluentValidation;

namespace Contract.Dtos.CourseResources.Requests;

public record CourseResourceCreateRequest
{
    public required string Title { get; init; }
    public required string Description { get; init; }
    public required FileType ResourceType { get; init; }
    public required string ResourceUrl { get; init; }
}

public class CourseResourceCreateRequestValidator : AbstractValidator<CourseResourceCreateRequest>
{
    public CourseResourceCreateRequestValidator()
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