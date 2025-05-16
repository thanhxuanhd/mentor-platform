using System.Net;
using Contract.Dtos.Courses.Requests;
using Contract.Dtos.Courses.Responses;
using Contract.Repositories;
using Contract.Services;
using Contract.Shared;
using Domain.Entities;
using Domain.Enums;

namespace Application.Services.Courses;

public class CourseService(ICourseRepository courseRepository) : ICourseService
{
    public async Task<Result<CourseListResponse>> GetAllAsync(CourseListRequest request)
    {
        if (request.PageIndex <= 0 || request.PageSize <= 0)
            return Result.Failure<CourseListResponse>("Page index and page size must be greater than 0",
                HttpStatusCode.BadRequest);

        var courses = await courseRepository.GetPaginatedCoursesAsync(request.PageIndex, request.PageSize,
            request.CategoryId, request.MentorId);

        var items = courses.Items.Select(c => new CourseSummary
        {
            Id = c.Id,
            Title = c.Title,
            Description = c.Description,
            CategoryId = c.CategoryId,
            CategoryName = c.Category?.Name,
            Difficulty = c.Difficulty,
            DueDate = c.DueDate,
            Status = c.Status
        }).ToList();

        var response = new CourseListResponse(items, courses.TotalCount, courses.PageIndex, courses.PageSize);

        return Result.Success(response, HttpStatusCode.OK);
    }

    public async Task<Result<CourseSummary>> GetByIdAsync(Guid id)
    {
        var course = await courseRepository.GetCourseWithDetailsAsync(id);
        if (course == null) return Result.Failure<CourseSummary>("Course not found", HttpStatusCode.NotFound);

        var response = new CourseSummary
        {
            Id = course.Id,
            Title = course.Title,
            Description = course.Description,
            CategoryId = course.CategoryId,
            CategoryName = course.Category?.Name,
            Difficulty = course.Difficulty,
            DueDate = course.DueDate,
            Status = course.Status
        };

        return Result.Success(response, HttpStatusCode.OK);
    }

    public async Task<Result<CourseSummary>> CreateAsync(CourseCreateRequest request)
    {
        var course = new Course
        {
            Title = request.Title,
            Description = request.Description,
            CategoryId = request.CategoryId,
            DueDate = request.DueDate,
            Status = CourseStatus.Draft,
            Difficulty = request.Difficulty
        };

        await courseRepository.AddAsync(course);
        await courseRepository.SaveChangesAsync();

        var response = new CourseSummary
        {
            Id = course.Id,
            Title = course.Title,
            Description = course.Description,
            CategoryId = course.CategoryId,
            Difficulty = course.Difficulty,
            DueDate = course.DueDate,
            Status = course.Status
        };

        return Result.Success(response, HttpStatusCode.Created);
    }

    public async Task<Result<CourseSummary>> UpdateAsync(Guid id, CourseUpdateRequest request)
    {
        var course = await courseRepository.GetByIdAsync(id);
        if (course == null) return Result.Failure<CourseSummary>("Course not found", HttpStatusCode.NotFound);

        course.Title = request.Title;
        course.Description = request.Description;
        course.CategoryId = request.CategoryId;
        course.DueDate = request.DueDate;
        course.Difficulty = request.Difficulty;

        await courseRepository.SaveChangesAsync();

        var response = new CourseSummary
        {
            Id = course.Id,
            Title = course.Title,
            Description = course.Description,
            CategoryId = course.CategoryId,
            Difficulty = course.Difficulty,
            DueDate = course.DueDate,
            Status = course.Status
        };

        return Result.Success(response, HttpStatusCode.OK);
    }

    public async Task<Result> DeleteAsync(Guid id)
    {
        var course = await courseRepository.GetByIdAsync(id);
        if (course == null) return Result.Failure("Course not found", HttpStatusCode.NotFound);

        courseRepository.Delete(course);
        await courseRepository.SaveChangesAsync();

        return Result.Success(HttpStatusCode.NoContent);
    }
}