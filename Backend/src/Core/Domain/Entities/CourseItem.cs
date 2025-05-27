using Domain.Abstractions;
using Domain.Enums;

namespace Domain.Entities;

public class CourseItem : BaseEntity<Guid>
{
    public required string Title { get; set; }
    public required string Description { get; set; }
    public FileType MediaType { get; set; }
    public string WebAddress { get; set; }

    public Guid CourseId { get; set; }
    public Course Course { get; set; } = null!;
}
