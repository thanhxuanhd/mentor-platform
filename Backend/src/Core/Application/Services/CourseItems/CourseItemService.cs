using Contract.Dtos.CourseItems.Requests;
using Contract.Dtos.Courses.Responses;
using Contract.Repositories;
using Contract.Shared;
using Domain.Entities;
using static System.Net.HttpStatusCode;

namespace Application.Services.CourseItems;

public class CourseItemService(ICourseItemRepository courseItemRepository, ICourseRepository courseRepository)
    : ICourseItemService
{
    public async Task<Result<List<CourseItemResponse>>> GetAllByCourseIdAsync(Guid courseId)
    {
        var course = await courseRepository.GetByIdAsync(courseId);
        if (course == null)
        {
            return Result.Failure<List<CourseItemResponse>>(
                $"Course with id = {courseId} not found",
                NotFound);
        }

        var query = courseItemRepository.GetAll()
            .OrderBy(ci => ci.Id)
            .Where(ci => ci.CourseId == courseId)
            .Select(i => i.ToCourseItemResponse());
        var items = await courseItemRepository.ToListAsync(query);

        return Result.Success(items, OK);
    }

    public async Task<Result<CourseItemResponse>> GetByIdAsync(Guid courseItemId)
    {
        var item = await courseItemRepository.GetByIdAsync(courseItemId);
        return item == null
            ? Result.Failure<CourseItemResponse>(
                $"Course item with id = {courseItemId} not found",
                NotFound)
            : Result.Success(item.ToCourseItemResponse(), OK);
    }

    public async Task<Result<CourseItemResponse>> CreateAsync(Guid courseId, CourseItemCreateRequest request)
    {
        var course = await courseRepository.GetByIdAsync(courseId);
        if (course == null)
        {
            return Result.Failure<CourseItemResponse>(
                $"Course with id = {courseId} not found",
                NotFound);
        }

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

        return Result.Success(item.ToCourseItemResponse(), Created);
    }

    public async Task<Result<CourseItemResponse>> UpdateAsync(Guid courseItemId,
        CourseItemUpdateRequest request)
    {
        var item = await courseItemRepository.GetByIdAsync(courseItemId);
        if (item == null)
        {
            return Result.Failure<CourseItemResponse>(
                $"Course item with id = {courseItemId} not found",
                NotFound);
        }

        item.Title = request.Title;
        item.Description = request.Description;
        item.MediaType = request.MediaType;
        item.WebAddress = request.WebAddress;

        await courseItemRepository.SaveChangesAsync();

        return Result.Success(item.ToCourseItemResponse(), OK);
    }

    public async Task<Result<bool>> DeleteAsync(Guid courseItemId)
    {
        var item = await courseItemRepository.GetByIdAsync(courseItemId);
        if (item == null)
        {
            return Result.Failure<bool>(
                $"Course item with id = {courseItemId} not found",
                NotFound);
        }

        courseItemRepository.Delete(item);
        await courseItemRepository.SaveChangesAsync();

        return Result.Success(true, OK);
    }
}