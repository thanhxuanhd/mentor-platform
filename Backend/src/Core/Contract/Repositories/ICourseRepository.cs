using Domain.Entities;

namespace Contract.Repositories;

public interface ICourseRepository : IBaseRepository<Course, Guid>
{
    Task LoadReferencedEntities(Course course);
    Task<Course?> GetByTitleAsync(string title);
    Task UpdateTagsCollection(List<Tag> tags, Course course);
}