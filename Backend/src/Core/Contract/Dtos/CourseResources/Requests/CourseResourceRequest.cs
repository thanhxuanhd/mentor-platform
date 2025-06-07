using FluentValidation;
using Microsoft.AspNetCore.Http;

namespace Contract.Dtos.CourseResources.Requests;

public record CourseResourceRequest
{
    public Guid CourseId { get; init; }
    public required string Title { get; init; }
    public string? Description { get; init; }
    public required IFormFile Resource { get; init; }
}

public class CourseResourceRequestValidator : AbstractValidator<CourseResourceRequest>
{
    public CourseResourceRequestValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage("Resource title can not be empty")
            .MaximumLength(50)
            .WithMessage("Title should not exceed 50 characters");

        RuleFor(x => x.Description)
            .MaximumLength(300)
            .WithMessage("Description should not exceed 300 characters");
    }
}