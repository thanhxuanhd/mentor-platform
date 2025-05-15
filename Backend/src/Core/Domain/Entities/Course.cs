using Domain.Abstractions;
using Domain.Enums;

namespace Domain.Entities;

public class Course : BaseEntity<Guid>
{
    public string Title { get; set; } = null!;
    public string Description { get; set; } = null!;
    public CourseStatus Status { get; set; }
    public DateTime DueDate { get; set; }
    public CourseDifficulty Difficulty { get; set; }
    public Guid CategoryId { get; set; }
    public Category Category { get; private set; } = null!;
    public Guid MentorId { get; set; }
    public User Mentor { get; private set; } = null!;
    public List<CourseTag> CourseTags { get; } = [];
    public List<Tag>? Tags { get; } = [];
}