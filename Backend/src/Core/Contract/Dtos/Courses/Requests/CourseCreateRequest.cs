using System;
using Domain.Enums;

namespace Contract.Dtos.Courses.Requests;

public class CourseCreateRequest
{
    public required string Title { get; set; }
    public required string Description { get; set; }
    public required Guid CategoryId { get; set; }
    public required DateTime DueDate { get; set; }
    public required CourseDifficulty Difficulty { get; set; }
}
