
using Contract.Dtos.CourseResources.Requests;
using Contract.Dtos.Courses.Responses;
using Contract.Shared;

namespace Application.Services.CourseResources;

public interface ICourseResourceService
{
    Task<Result<List<CourseResourceResponse>>> GetAllByCourseIdAsync(Guid courseId);
    Task<Result<CourseResourceResponse>> GetByIdAsync(Guid courseResourceId);
    Task<Result<CourseResourceResponse>> CreateAsync(Guid courseId, CourseResourceCreateRequest request);
    Task<Result<CourseResourceResponse>> UpdateAsync(Guid courseResourceId, CourseResourceUpdateRequest request);
    Task<Result<bool>> DeleteAsync(Guid courseResourceId);
}