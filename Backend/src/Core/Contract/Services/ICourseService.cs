using Contract.Dtos.Courses.Requests;
using Contract.Dtos.Courses.Responses;
using Contract.Shared;

namespace Contract.Services;

public interface ICourseService
{
    Task<Result<CourseListResponse>> GetAllAsync(CourseListRequest request);
    Task<Result<CourseSummary>> GetByIdAsync(Guid id);
    Task<Result<CourseSummary>> CreateAsync(CourseCreateRequest request);
    Task<Result<CourseSummary>> UpdateAsync(Guid id, CourseUpdateRequest request);
    Task<Result> DeleteAsync(Guid id);
}
