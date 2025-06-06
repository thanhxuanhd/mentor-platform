using Domain.Enums;

namespace Contract.Dtos.CourseResources.Responses;

public record CourseResourceResponse
{
    public Guid Id { get; init; }
    public required string Title { get; init; }
    public string? Description { get; init; }
    public FileType ResourceType { get; init; }
    public required string ResourceUrl { get; init; }
    public Guid CourseId { get; init; }
    public Guid MentorId { get; init; }
    public required string CourseTitle { get; init; }
}