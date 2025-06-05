using Application.Helpers;
using Contract.Dtos.CourseResources.Requests;
using Contract.Dtos.CourseResources.Responses;
using Contract.Dtos.Courses.Responses;
using Contract.Repositories;
using Contract.Shared;
using Domain.Constants;
using Domain.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.Net;
using static System.Net.HttpStatusCode;

namespace Application.Services.CourseResources;

public class CourseResourceService(ICourseResourceRepository courseResourceRepository, ICourseRepository courseRepository, IWebHostEnvironment env)
    : ICourseResourceService
{
    public async Task<Result<List<CourseResourceResponse>>> GetAllByCourseIdAsync(Guid courseId)
    {
        var course = await courseRepository.GetByIdAsync(courseId);
        if (course == null)
        {
            return Result.Failure<List<CourseResourceResponse>>(
                $"Course with id = {courseId} not found",
                NotFound);
        }

        var query = courseResourceRepository.GetAll()
            .OrderBy(ci => ci.Id)
            .Where(ci => ci.CourseId == courseId)
            .Select(i => i.ToCourseResourceResponse());
        var resources = await courseResourceRepository.ToListAsync(query);

        return Result.Success(resources, OK);
    }

    public async Task<Result<CourseResourceResponse>> GetByIdAsync(Guid courseResourceId)
    {
        var resource = await courseResourceRepository.GetByIdAsync(courseResourceId);
        return resource == null
            ? Result.Failure<CourseResourceResponse>(
                $"Course resource with id = {courseResourceId} not found",
                NotFound)
            : Result.Success(resource.ToCourseResourceResponse(), OK);
    }

    public async Task<Result<CourseResourceResponse>> CreateAsync(Guid mentorId, Guid courseId, CourseResourceRequest formData, HttpRequest httpRequest)
    {
        var course = await courseRepository.GetByIdAsync(courseId, c => c.Resources);
        if (course == null)
        {
            return Result.Failure<CourseResourceResponse>(
                $"Course with id = {courseId} not found",
                NotFound);
        }

        if (course.Resources.Count >= 5)
        {
            return Result.Failure<CourseResourceResponse>(
                $"You can only upload max 5 materials",
                BadRequest);
        }

        if (mentorId != course.MentorId)
        {
            return Result.Failure<CourseResourceResponse>(
                $"You are not allowed to upload resources to this course",
                Forbidden);
        }

        var uploadResult = await UploadFileAsync(courseId, formData.Resource, httpRequest);
        if (!uploadResult.IsSuccess)
        {
            return Result.Failure<CourseResourceResponse>(uploadResult.Error, uploadResult.StatusCode);
        }

        var resource = new CourseResource
        {
            Title = formData.Title,
            Description = formData.Description,
            CourseId = courseId,
            Course = course,
            ResourceUrl = uploadResult.Value.fileUrl,
            ResourceType = FileHelper.GetFileTypeFromUrl(uploadResult.Value.fileUrl)
        };

        await courseResourceRepository.AddAsync(resource);
        await courseResourceRepository.SaveChangesAsync();

        return Result.Success(resource.ToCourseResourceResponse(), Created);
    }

    public async Task<Result<CourseResourceResponse>> UpdateAsync(Guid mentorId, Guid courseResourceId,
        CourseResourceRequest formData, HttpRequest httpRequest)
    {
        var resource = await courseResourceRepository.GetByIdAsync(courseResourceId);
        if (resource == null)
        {
            return Result.Failure<CourseResourceResponse>(
                $"Course resource with id = {courseResourceId} not found",
                NotFound);
        }

        var courseId = formData.CourseId;
        var course = await courseRepository.GetByIdAsync(courseId);
        if (course == null)
        {
            return Result.Failure<CourseResourceResponse>(
                $"Course with id = {courseId} not found",
                NotFound);
        }

        if (mentorId != course.MentorId)
        {
            return Result.Failure<CourseResourceResponse>(
                $"You are not allowed to edit resources from this course",
                Forbidden);
        }

        resource.Title = formData.Title;
        resource.Description = formData.Description;
        if (resource.CourseId != courseId)
        {
            return Result.Failure<CourseResourceResponse>("You can not change course of existing resource. Please delete and add new", HttpStatusCode.BadRequest);
        }
        var uploadResult = await UploadFileAsync(courseId, formData.Resource, httpRequest);
        if (!uploadResult.IsSuccess)
        {
            return Result.Failure<CourseResourceResponse>(uploadResult.Error, uploadResult.StatusCode);
        }

        resource.CourseId = courseId;
        resource.Course = course;

        var deletionResult = DeleteFile(resource.ResourceUrl, resource.CourseId);
        if (!deletionResult.IsSuccess)
        {
            return Result.Failure<CourseResourceResponse>(deletionResult.Error, deletionResult.StatusCode);
        }

        resource.ResourceUrl = uploadResult.Value.fileUrl;
        resource.ResourceType = FileHelper.GetFileTypeFromUrl(uploadResult.Value.fileUrl);

        courseResourceRepository.Update(resource);
        await courseResourceRepository.SaveChangesAsync();

        return Result.Success(resource.ToCourseResourceResponse(), OK);
    }

    public async Task<Result<bool>> DeleteAsync(Guid mentorId, Guid courseResourceId)
    {
        var resource = await courseResourceRepository.GetByIdAsync(courseResourceId, cr => cr.Course);
        if (resource == null)
        {
            return Result.Failure<bool>(
                $"Course resource with id = {courseResourceId} not found",
                NotFound);
        }

        if (resource.Course.MentorId != mentorId)
        {
            return Result.Failure<bool>(
                $"You are not allowed to edit resources from this course",
                Forbidden);
        }

        var deletionResult = DeleteFile(resource.ResourceUrl, resource.CourseId);
        if (!deletionResult.IsSuccess)
        {
            return Result.Failure<bool>(deletionResult.Error, deletionResult.StatusCode);
        }

        courseResourceRepository.Delete(resource);
        await courseResourceRepository.SaveChangesAsync();

        return Result.Success(true, OK);
    }

    public async Task<Result<PaginatedList<CourseResourceResponse>>> FilterResourceAsync(FilterResourceRequest request)
    {
        var resources = courseResourceRepository.GetAll();

        if (!string.IsNullOrWhiteSpace(request.Keyword))
        {
            resources = resources.Where(c => c.Title.Contains(request.Keyword));
        }
        
        if (request.ResourceType.HasValue)
        {
            resources = resources.Where(c => c.ResourceType == request.ResourceType);
        }

        var filteredResources = resources.Select(
            r => new CourseResourceResponse
            {
                Id = r.Id,
                Title = r.Title,
                Description = r.Description,
                ResourceType = r.ResourceType,
                ResourceUrl = r.ResourceUrl,
                CourseId = r.CourseId,
                CourseTitle = r.Course.Title
            });

        var paginatedResources = await courseResourceRepository
            .ToPaginatedListAsync(
                filteredResources,
                request.PageSize,
                request.PageIndex);

        return Result.Success(paginatedResources, HttpStatusCode.OK);
    }

    private async Task<Result<(string fileUrl, string filePath)>> UploadFileAsync(Guid courseId, IFormFile file, HttpRequest httpRequest)
    {
        if (file == null || file.Length == 0)
        {
            return Result.Failure<(string, string)>("File not selected", HttpStatusCode.BadRequest);
        }

        var fileContentType = file.ContentType;
        if (!FileConstants.FILE_CONTENT_TYPES.Contains(fileContentType))
        {
            return Result.Failure<(string, string)>("File content type is not allowed.", HttpStatusCode.BadRequest);
        }

        if (file.Length > FileConstants.MAX_FILE_SIZE)
        {
            return Result.Failure<(string, string)>("File should not exceed 1 MB", HttpStatusCode.BadRequest);
        }

        var path = Directory.GetCurrentDirectory();
        var resourcesPath = Path.Combine(path, env.WebRootPath, "resources", $"{courseId}");
        if (!Directory.Exists(resourcesPath))
        {
            Directory.CreateDirectory(resourcesPath);
        }

        var filePath = Path.Combine(resourcesPath, file.FileName);
        var baseUrl = $"{httpRequest?.Scheme}://{httpRequest?.Host}";
        string fileUrl = $"{baseUrl}/resources/{courseId}/{file.FileName}";

        var existingResource = courseResourceRepository.GetAll()
            .FirstOrDefault(r => r.CourseId == courseId && r.ResourceUrl == fileUrl);

        if (existingResource != null)
        {
            return Result.Failure<(string, string)>("A file with this name already exists for this course.", HttpStatusCode.BadRequest);
        }

        try
        {
            using var stream = new FileStream(filePath, FileMode.Create);
            await file.CopyToAsync(stream);
            return Result.Success((fileUrl, filePath), OK);
        }
        catch (Exception ex)
        {
            return Result.Failure<(string, string)>($"Failed to save file: {ex.Message}", HttpStatusCode.InternalServerError);
        }
    }

    private Result DeleteFile(string resourceUrl, Guid courseId)
    {
        if (string.IsNullOrEmpty(resourceUrl))
        {
            return Result.Success(OK);
        }

        try
        {
            if (Uri.TryCreate(resourceUrl, UriKind.Absolute, out var uri) && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps))
            {
                var fileName = Path.GetFileName(uri.LocalPath);
                var path = Directory.GetCurrentDirectory();
                var resourcesPath = Path.Combine(path, env.WebRootPath, "resources", $"{courseId}");
                var filePath = Path.Combine(resourcesPath, fileName);

                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
            }
            return Result.Success(OK);
        }
        catch (Exception ex)
        {
            return Result.Failure($"Failed to delete file: {ex.Message}", HttpStatusCode.InternalServerError);
        }
    }
}