using Contract.Dtos.Courses.Responses;
using Contract.Shared;
using Domain.Entities;
using Domain.Enums;

namespace Contract.Repositories;

public interface ICourseRepository : IBaseRepository<Course, Guid>
{
    Task UpdateTagsCollection(List<Tag> tags, Course course);

    Task<Course?> GetCourseWithDetailsAsync(Guid id);

    Task<PaginatedList<CourseSummary>> GetPaginatedCoursesAsync(int pageIndex,
        int pageSize,
        Guid? categoryId = null,
        Guid? mentorId = null,
        string? keyword = null,
        CourseStatus? status = null,
        CourseDifficulty? difficulty = null);

    Task<Course?> GetCourseByTitleAsync(string title);
}