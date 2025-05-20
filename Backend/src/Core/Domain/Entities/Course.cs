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
    public Category Category { get; set; } = null!;
    public Guid MentorId { get; set; }
    public User Mentor { get; set; } = null!;
    public List<CourseTag> CourseTags { get; set; }
    public List<Tag> Tags { get; } = [];
    public List<CourseItem> Items { get; } = [];
}