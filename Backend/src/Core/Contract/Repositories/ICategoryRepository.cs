using Domain.Entities;

namespace Contract.Repositories;

public interface ICategoryRepository : IBaseRepository<Category, Guid>
{
    Task<bool> ExistByNameAsync(string name);
    IQueryable<Course> FilterCourseByCategory(Guid id);
}
