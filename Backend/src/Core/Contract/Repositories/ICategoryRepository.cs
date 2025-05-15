using Domain.Entities;

namespace Contract.Repositories;

public interface ICategoryRepository : IBaseRepository<Category, Guid>
{
    Task<bool> ExistByNameAsync(string name);
    Task<bool> ExistByNameExcludeAsync(Guid id, string name);
    IQueryable<Course> FilterCourseByCategory(Guid id);
}
