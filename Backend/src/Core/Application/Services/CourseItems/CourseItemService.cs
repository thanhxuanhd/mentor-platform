using System.Net;
using Contract.Dtos.CourseItems.Requests;
using Contract.Dtos.Courses.Responses;
using Contract.Repositories;
using Contract.Services;
using Contract.Shared;
using Domain.Entities;

namespace Application.Services.CourseItems;

public class CourseItemService(ICourseItemRepository courseItemRepository, ICourseRepository courseRepository)
    : ICourseItemService
{
    public async Task<Result<List<CourseItemDto>>> GetAllByCourseIdAsync(Guid courseId)
    {
        var course = await courseRepository.GetByIdAsync(courseId);
        if (course == null)
            return Result.Failure<List<CourseItemDto>>("Course not found", HttpStatusCode.NotFound);

        var items = await courseItemRepository.GetAllByCourseIdAsync(courseId);
        var itemDtos = items.Select(i => i.ToCourseItemDto()).ToList();

        return Result.Success(itemDtos, HttpStatusCode.OK);
    }

    public async Task<Result<CourseItemDto>> GetByIdAsync(Guid courseId, Guid resourceId)
    {
        var course = await courseRepository.GetByIdAsync(courseId);
        if (course == null)
            return Result.Failure<CourseItemDto>("Course not found", HttpStatusCode.NotFound);

        var item = await courseItemRepository.GetByIdAsync(courseId, resourceId);
        if (item == null)
            return Result.Failure<CourseItemDto>("Resource not found", HttpStatusCode.NotFound);

        return Result.Success(item.ToCourseItemDto(), HttpStatusCode.OK);
    }

    public async Task<Result<CourseItemDto>> CreateAsync(Guid courseId, CourseItemCreateRequest request)
    {
        var course = await courseRepository.GetByIdAsync(courseId);
        if (course == null)
            return Result.Failure<CourseItemDto>("Course not found", HttpStatusCode.NotFound);

        var item = new CourseItem
        {
            Title = request.Title,
            Description = request.Description,
            MediaType = request.MediaType,
            WebAddress = request.WebAddress,
            CourseId = courseId
        };

        await courseItemRepository.AddAsync(item);
        await courseItemRepository.SaveChangesAsync();

        return Result.Success(item.ToCourseItemDto(), HttpStatusCode.Created);
    }

    public async Task<Result<CourseItemDto>> UpdateAsync(Guid courseId, Guid resourceId,
        CourseItemUpdateRequest request)
    {
        var course = await courseRepository.GetByIdAsync(courseId);
        if (course == null)
            return Result.Failure<CourseItemDto>("Course not found", HttpStatusCode.NotFound);

        var item = await courseItemRepository.GetByIdAsync(courseId, resourceId);
        if (item == null)
            return Result.Failure<CourseItemDto>("Resource not found", HttpStatusCode.NotFound);

        item.Title = request.Title;
        item.Description = request.Description;
        item.MediaType = request.MediaType;
        item.WebAddress = request.WebAddress;

        await courseItemRepository.SaveChangesAsync();

        return Result.Success(item.ToCourseItemDto(), HttpStatusCode.OK);
    }

    public async Task<Result<bool>> DeleteAsync(Guid courseId, Guid resourceId)
    {
        var course = await courseRepository.GetByIdAsync(courseId);
        if (course == null)
            return Result.Failure<bool>("Course not found", HttpStatusCode.NotFound);

        var item = await courseItemRepository.GetByIdAsync(courseId, resourceId);
        if (item == null)
            return Result.Failure<bool>("Resource not found", HttpStatusCode.NotFound);

        courseItemRepository.Delete(item);
        await courseItemRepository.SaveChangesAsync();

        return Result.Success(true, HttpStatusCode.OK);
    }
}