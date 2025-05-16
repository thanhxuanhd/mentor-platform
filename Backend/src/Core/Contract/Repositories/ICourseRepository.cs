using Contract.Shared;
using Domain.Entities;
using Domain.Enums;

namespace Contract.Repositories;

public interface ICourseRepository : IBaseRepository<Course, Guid>
{
    Task<Course?> GetCourseWithDetailsAsync(Guid id);

    Task<PaginatedList<Course>> GetPaginatedCoursesAsync(int pageIndex,
        int pageSize,
        Guid? categoryId = null,
        Guid? mentorId = null,
        string? keyword = null,
        CourseState? status = null);
}