using Domain.Enums;

namespace Contract.Dtos.CourseResources.Responses;

public record CourseResourceDto
{
    public Guid Id { get; init; }
    public required string Title { get; init; }
    public required string Description { get; init; }
    public required FileType ResourceType { get; init; }
    public required string ResourceUrl { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
}