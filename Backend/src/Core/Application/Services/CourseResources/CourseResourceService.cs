using Contract.Dtos.CourseResources.Requests;
using Contract.Dtos.Courses.Responses;
using Contract.Repositories;
using Contract.Shared;
using Domain.Entities;
using static System.Net.HttpStatusCode;

namespace Application.Services.CourseResources;

public class CourseResourceService(ICourseResourceRepository courseResourceRepository, ICourseRepository courseRepository)
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
        var items = await courseResourceRepository.ToListAsync(query);

        return Result.Success(items, OK);
    }

    public async Task<Result<CourseResourceResponse>> GetByIdAsync(Guid courseResourceId)
    {
        var item = await courseResourceRepository.GetByIdAsync(courseResourceId);
        return item == null
            ? Result.Failure<CourseResourceResponse>(
                $"Course item with id = {courseResourceId} not found",
                NotFound)
            : Result.Success(item.ToCourseResourceResponse(), OK);
    }

    public async Task<Result<CourseResourceResponse>> CreateAsync(Guid courseId, CourseResourceCreateRequest request)
    {
        var course = await courseRepository.GetByIdAsync(courseId);
        if (course == null)
        {
            return Result.Failure<CourseResourceResponse>(
                $"Course with id = {courseId} not found",
                NotFound);
        }

        var item = new CourseResource
        {
            Title = request.Title,
            Description = request.Description,
            ResourceType = request.ResourceType,
            ResourceUrl = request.ResourceUrl,
            CourseId = courseId
        };

        await courseResourceRepository.AddAsync(item);
        await courseResourceRepository.SaveChangesAsync();

        return Result.Success(item.ToCourseResourceResponse(), Created);
    }

    public async Task<Result<CourseResourceResponse>> UpdateAsync(Guid courseResourceId,
        CourseResourceUpdateRequest request)
    {
        var item = await courseResourceRepository.GetByIdAsync(courseResourceId);
        if (item == null)
        {
            return Result.Failure<CourseResourceResponse>(
                $"Course item with id = {courseResourceId} not found",
                NotFound);
        }

        item.Title = request.Title;
        item.Description = request.Description;
        item.ResourceType = request.ResourceType;
        item.ResourceUrl = request.ResourceUrl;

        await courseResourceRepository.SaveChangesAsync();

        return Result.Success(item.ToCourseResourceResponse(), OK);
    }

    public async Task<Result<bool>> DeleteAsync(Guid courseResourceId)
    {
        var item = await courseResourceRepository.GetByIdAsync(courseResourceId);
        if (item == null)
        {
            return Result.Failure<bool>(
                $"Course item with id = {courseResourceId} not found",
                NotFound);
        }

        courseResourceRepository.Delete(item);
        await courseResourceRepository.SaveChangesAsync();

        return Result.Success(true, OK);
    }
}