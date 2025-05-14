using Domain.Entities;

namespace Contract.Repositories;

public interface ICategoryRepository : IBaseRepository<Category, Guid>
{
    IQueryable<Course> FilterCourseByCategory(Guid id);
}
