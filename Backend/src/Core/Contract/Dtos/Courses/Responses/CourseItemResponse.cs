using Domain.Enums;

namespace Contract.Dtos.Courses.Responses;

public record CourseItemResponse
{
    public Guid Id { get; init; }
    public required string Title { get; init; }
    public required string Description { get; init; }
    public CourseMediaType MediaType { get; init; }
    public string WebAddress { get; init; }
}