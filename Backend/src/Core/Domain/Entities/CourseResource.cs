using Domain.Abstractions;
using Domain.Enums;

namespace Domain.Entities;

public class CourseResource : BaseEntity<Guid>
{
    public required string Title { get; set; }
    public required string Description { get; set; }
    public FileType ResourceType { get; set; }
    public required string ResourceUrl { get; set; }
    public Guid CourseId { get; set; }
    public Course Course { get; set; } = null!;
}