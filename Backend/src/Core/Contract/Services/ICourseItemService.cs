using Contract.Dtos.CourseItems.Requests;
using Contract.Dtos.Courses.Responses;
using Contract.Shared;

namespace Contract.Services;

public interface ICourseItemService
{
    Task<Result<List<CourseItemResponse>>> GetAllByCourseIdAsync(Guid courseId);
    Task<Result<CourseItemResponse>> GetByIdAsync(Guid courseId, Guid resourceId);
    Task<Result<CourseItemResponse>> CreateAsync(Guid courseId, CourseItemCreateRequest request);
    Task<Result<CourseItemResponse>> UpdateAsync(Guid courseId, Guid resourceId, CourseItemUpdateRequest request);
    Task<Result<bool>> DeleteAsync(Guid courseId, Guid resourceId);
}
