using Domain.Abstractions;
using Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
    public class Course : BaseEntity<uint>
    {
        public string Title { get; set; } = null!;
        public uint? CategoryId { get; set; }
        public CourseStatus Status { get; set; } = CourseStatus.Draft;
        public DateTime? Duration { get; set; }
        public string? Description { get; set; }
        public CourseDifficulty Difficulty { get; set; } = CourseDifficulty.Beginner;
        public virtual Category? Category { get; set; }
        public virtual ICollection<CourseTag>? CourseTags { get; set; }
    }
}
