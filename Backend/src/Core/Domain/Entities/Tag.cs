using Domain.Abstractions;

namespace Domain.Entities
{
    public class Tag : BaseEntity<Guid>
    {
        public string Name { get; set; } = null!;
        public virtual ICollection<CourseTag> CourseTags { get; set; } = new HashSet<CourseTag>();
    }
}
