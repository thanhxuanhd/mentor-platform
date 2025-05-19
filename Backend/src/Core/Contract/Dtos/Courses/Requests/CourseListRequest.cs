using System.ComponentModel;
using Domain.Enums;
using FluentValidation;

namespace Contract.Dtos.Courses.Requests;

public record CourseListRequest
{
    public int PageIndex { get; init; }
    public int PageSize { get; init; }
    public Guid? CategoryId { get; init; }
    public Guid? MentorId { get; init; }
    public string? Keyword { get; init; }
    public CourseStatus? Status { get; init; }
    public CourseDifficulty? Difficulty { get; init; }
}

public class CourseListRequestValidator : AbstractValidator<CourseListRequest>
{
    public CourseListRequestValidator()
    {
        RuleFor(request => request.PageIndex)
            .NotNull()
            .WithMessage("PageIndex is required")
            .GreaterThan(0)
            .WithMessage("PageIndex must be greater than 0");
        
        RuleFor(request => request.PageSize)
            .NotNull()
            .WithMessage("PageSize is required")
            .GreaterThan(0)
            .WithMessage("PageSize must be greater than 0");
        
        RuleFor(x => x.Keyword)
            .MaximumLength(256)
            .WithMessage("Keyword must not exceed 256 characters.");
    }
}