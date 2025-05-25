using Contract.Dtos.Courses.Requests;
using Contract.Dtos.Courses.Responses;
using Contract.Shared;
using Domain.Enums;

namespace Application.Services.Courses;

public interface ICourseService
{
    Task<Result<PaginatedList<CourseSummaryResponse>>> GetAllAsync(Guid userId, UserRole userRole,
        CourseListRequest request);
    Task<Result<CourseSummaryResponse>> GetByIdAsync(Guid id);
    Task<Result<CourseSummaryResponse>> CreateAsync(Guid mentorId, CourseCreateRequest request);
    Task<Result<CourseSummaryResponse>> UpdateAsync(Guid id, CourseUpdateRequest request);
    Task<Result<CourseSummaryResponse>> PublishCourseAsync(Guid id);
    Task<Result<CourseSummaryResponse>> ArchiveCourseAsync(Guid id);
    Task<Result<bool>> DeleteAsync(Guid id);
}