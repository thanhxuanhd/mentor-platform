using Domain.Enums;

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