using Domain.Enums;

namespace Contract.Dtos.CourseItems.Responses;

public record CourseItemDto
{
    public Guid Id { get; init; }
    public required string Title { get; init; }
    public required string Description { get; init; }
    public required CourseMediaType MediaType { get; init; }
    public required string WebAddress { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
}
