using Domain.Enums;
using FluentValidation;

namespace Contract.Dtos.Courses.Requests;

public record CourseCreateRequest
{
    public required string Title { get; init; }
    public required string Description { get; init; }
    public required Guid CategoryId { get; init; }
    public required CourseDifficulty Difficulty { get; init; }
    public List<string> Tags { get; init; } = [];
    public required DateTime DueDate { get; init; }
}

public class CourseCreateRequestValidator : AbstractValidator<CourseCreateRequest>
{
    public CourseCreateRequestValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage("Please enter course title.")
            .MaximumLength(256)
            .WithMessage("Course title should not exceed 256 characters.");

        RuleFor(x => x.Description)
            .MaximumLength(256)
            .WithMessage("Description must not exceed 256 characters.");

        RuleFor(x => x.CategoryId)
            .NotEmpty()
            .WithMessage("Category Id is required.");

        RuleFor(x => x.DueDate)
            .Must(dueDate => DateOnly.FromDateTime(dueDate).CompareTo(DateOnly.FromDateTime(DateTime.Now)) >= 0)
            .WithMessage("Due Date must be in the future.");

        RuleFor(x => x.Tags)
            .Must(tags => tags.Count <= 5)
            .WithMessage("You can choose maximum 5 tags.");
    }
}