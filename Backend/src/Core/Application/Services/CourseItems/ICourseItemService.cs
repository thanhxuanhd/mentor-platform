using Contract.Dtos.CourseItems.Requests;
using Contract.Dtos.Courses.Responses;
using Contract.Shared;

namespace Application.Services.CourseItems;

public interface ICourseItemService
{
    Task<Result<List<CourseItemResponse>>> GetAllByCourseIdAsync(Guid courseId);
    Task<Result<CourseItemResponse>> GetByIdAsync(Guid courseItemId);
    Task<Result<CourseItemResponse>> CreateAsync(Guid courseId, CourseItemCreateRequest request);
    Task<Result<CourseItemResponse>> UpdateAsync(Guid courseItemId, CourseItemUpdateRequest request);
    Task<Result<bool>> DeleteAsync(Guid courseItemId);
}