
using Contract.Dtos.CourseResources.Requests;
using Contract.Dtos.CourseResources.Responses;
using Contract.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Application.Services.CourseResources;

public interface ICourseResourceService
{
    Task<Result<List<CourseResourceResponse>>> GetAllByCourseIdAsync(Guid courseId);
    Task<Result<CourseResourceResponse>> GetByIdAsync(Guid courseResourceId);
    Task<Result<CourseResourceResponse>> CreateAsync(Guid mentorId, Guid courseId, CourseResourceRequest formData, HttpRequest httpRequest);
    Task<Result<CourseResourceResponse>> UpdateAsync(Guid mentorId, Guid courseResourceId, CourseResourceRequest formData, HttpRequest httpRequest);
    Task<Result<bool>> DeleteAsync(Guid mentorId, Guid courseResourceId);
    Task<Result<PaginatedList<CourseResourceResponse>>> FilterResourceAsync(FilterResourceRequest request);
    Task<FileResult> DownloadFileAsync(Guid courseResourceId, string fileName);
}