using Domain.Enums;

namespace Contract.Dtos.Courses.Requests;

public record CourseCreateRequest
{
    public required string Title { get; init; }
    public required string Description { get; init; }
    public required Guid CategoryId { get; init; }
    public required CourseDifficulty Difficulty { get; init; }
    public List<string> Tags { get; init; } = [];
    public required DateTime DueDate { get; init; }
    public required Guid MentorId { get; init; }
}