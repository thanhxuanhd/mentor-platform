using Domain.Enums;

namespace Contract.Dtos.CourseItems.Requests;

public record CourseItemCreateRequest
{
    public required string Title { get; init; }
    public required string Description { get; init; }
    public required CourseMediaType MediaType { get; init; }
    public required string WebAddress { get; init; }
}
