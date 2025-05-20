using System.Net;
using Contract.Dtos.Courses.Requests;
using Contract.Dtos.Courses.Responses;
using Contract.Repositories;
using Contract.Services;
using Contract.Shared;
using Domain.Entities;
using Domain.Enums;

namespace Application.Services.Courses;

public class CourseService(ICourseRepository courseRepository, ITagRepository tagRepository) : ICourseService
{
    public async Task<Result<PaginatedList<CourseSummary>>> GetAllAsync(CourseListRequest request)
    {
        if (request.PageIndex <= 0 || request.PageSize <= 0)
            return Result.Failure<PaginatedList<CourseSummary>>("Page index and page size must be greater than 0",
                HttpStatusCode.BadRequest);

        var courses = await courseRepository.GetPaginatedCoursesAsync(
            request.PageIndex,
            request.PageSize,
            request.CategoryId,
            request.MentorId,
            request.Keyword,
            request.Status,
            request.Difficulty);

        return Result.Success(courses, HttpStatusCode.OK);
    }

    public async Task<Result<CourseSummary>> GetByIdAsync(Guid id)
    {
        var course = await courseRepository.GetCourseWithDetailsAsync(id);
        if (course == null) return Result.Failure<CourseSummary>("Course not found", HttpStatusCode.NotFound);

        var response = course.ToCourseSummary();
        return Result.Success(response, HttpStatusCode.OK);
    }

    public async Task<Result<CourseSummary>> CreateAsync(CourseCreateRequest request)
    {
        var normalizedTagNames = request.Tags.Select(t => char.ToUpper(t[0]) + t[1..].ToLower()).ToHashSet();
        var tags = await tagRepository.UpsertAsync(normalizedTagNames);
        await tagRepository.SaveChangesAsync();

        // TODO: Not checking MentorId contains role: Mentor, Tags not being processed in stores
        var course = new Course
        {
            Title = request.Title,
            Description = request.Description,
            CategoryId = request.CategoryId,
            MentorId = request.MentorId,
            DueDate = request.DueDate,
            Status = CourseStatus.Draft,
            Difficulty = request.Difficulty
        };

        await courseRepository.AddAsync(course);
        await courseRepository.UpdateTagsCollection(tags, course);

        await courseRepository.SaveChangesAsync();

        var createdCourse = await courseRepository.GetCourseWithDetailsAsync(course.Id);
        if (createdCourse == null)
            return Result.Failure<CourseSummary>("Failed to retrieve created course",
                HttpStatusCode.InternalServerError);

        var response = createdCourse.ToCourseSummary();

        return Result.Success(response, HttpStatusCode.Created);
    }

    public async Task<Result<CourseSummary>> UpdateAsync(Guid id, CourseUpdateRequest request)
    {
        var course = await courseRepository.GetByIdAsync(id);
        if (course == null) return Result.Failure<CourseSummary>("Course not found", HttpStatusCode.NotFound);

        var normalizedTagNames = request.Tags.Select(t => char.ToUpper(t[0]) + t[1..].ToLower()).ToHashSet();
        var tags = await tagRepository.UpsertAsync(normalizedTagNames);
        await tagRepository.SaveChangesAsync();

        course.Title = request.Title;
        course.Description = request.Description;
        course.CategoryId = request.CategoryId;
        course.DueDate = request.DueDate;
        course.Difficulty = request.Difficulty;

        await courseRepository.UpdateTagsCollection(tags, course);
        await courseRepository.SaveChangesAsync();

        var updatedCourse = await courseRepository.GetCourseWithDetailsAsync(id);
        if (updatedCourse == null)
            return Result.Failure<CourseSummary>("Failed to retrieve updated course",
                HttpStatusCode.InternalServerError);

        var response = course.ToCourseSummary();

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