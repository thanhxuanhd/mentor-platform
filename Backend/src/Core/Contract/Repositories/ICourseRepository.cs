using Contract.Shared;
using Domain.Entities;

namespace Contract.Repositories;

public interface ICourseRepository : IBaseRepository<Course, Guid>
{
    Task<Course?> GetCourseWithDetailsAsync(Guid id);
    Task<PaginatedList<Course>> GetPaginatedCoursesAsync(int pageIndex, int pageSize, Guid? categoryId = null, Guid? mentorId = null);
}
