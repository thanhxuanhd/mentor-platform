using Contract.Dtos.CourseItems.Requests;
using Contract.Dtos.Courses.Responses;
using Contract.Shared;

namespace Contract.Services;

public interface ICourseItemService
{
    Task<Result<List<CourseItemDto>>> GetAllByCourseIdAsync(Guid courseId);
    Task<Result<CourseItemDto>> GetByIdAsync(Guid courseId, Guid resourceId);
    Task<Result<CourseItemDto>> CreateAsync(Guid courseId, CourseItemCreateRequest request);
    Task<Result<CourseItemDto>> UpdateAsync(Guid courseId, Guid resourceId, CourseItemUpdateRequest request);
    Task<Result> DeleteAsync(Guid courseId, Guid resourceId);
}
