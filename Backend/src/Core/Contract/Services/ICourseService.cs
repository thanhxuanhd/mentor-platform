using Contract.Dtos.Courses.Requests;
using Contract.Dtos.Courses.Responses;
using Contract.Shared;

namespace Contract.Services;

public interface ICourseService
{
    Task<Result<PaginatedList<CourseSummary>>> GetAllAsync(CourseListRequest request);
    Task<Result<CourseSummary>> GetByIdAsync(Guid id);
    Task<Result<CourseSummary>> CreateAsync(Guid mentorId, CourseCreateRequest request);
    Task<Result<CourseSummary>> UpdateAsync(Guid id, CourseUpdateRequest request);
    Task<Result> DeleteAsync(Guid id);
    Task<Result<CourseSummary>> PublishCourseAsync(Guid id);
    Task<Result<CourseSummary>> ArchiveCourseAsync(Guid id);
}