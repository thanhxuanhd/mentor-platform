using Contract.Dtos.CourseResources.Responses;
using Domain.Enums;

namespace Contract.Dtos.Courses.Responses;

public class CourseSummaryResponse
{
    public Guid Id { get; set; }
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public Guid? CategoryId { get; set; }
    public string? CategoryName { get; set; }
    public Guid? MentorId { get; set; }
    public string? MentorName { get; set; }
    public CourseDifficulty Difficulty { get; set; }
    public DateTime? DueDate { get; set; }
    public List<CourseResourceResponse> Resources { get; init; } = [];
    public List<string> Tags { get; init; } = [];
    public CourseStatus Status { get; set; }
}