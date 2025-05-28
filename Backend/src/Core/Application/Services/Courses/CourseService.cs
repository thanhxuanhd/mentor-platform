using Contract.Dtos.Courses.Requests;
using Contract.Dtos.Courses.Responses;
using Contract.Repositories;
using Contract.Shared;
using Domain.Entities;
using Domain.Enums;
using static System.Net.HttpStatusCode;

namespace Application.Services.Courses;

public class CourseService(
    ICourseRepository courseRepository,
    ITagRepository tagRepository,
    ICategoryRepository categoryRepository) : ICourseService
{
    public async Task<Result<PaginatedList<CourseSummaryResponse>>> GetAllAsync(Guid userId, UserRole userRole,
        CourseListRequest request)
    {
        var (pageIndex, pageSize, categoryId, mentorId, keyword, status, difficulty) = request;

        var effectiveStatus = userRole == UserRole.Learner && status == CourseStatus.Draft
            ? CourseStatus.Published
            : status;

        var query = courseRepository.GetAll();

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            query = query.Where(c => c.Title.Contains(keyword) || c.Description.Contains(keyword));
        }

        if (categoryId.HasValue)
        {
            query = query.Where(c => c.CategoryId == categoryId);
        }

        if (difficulty.HasValue)
        {
            query = query.Where(c => c.Difficulty == difficulty);
        }

        if (effectiveStatus.HasValue)
        {
            query = query.Where(c => c.Status == effectiveStatus);
        }
        else if (userRole == UserRole.Learner)
        {
            query = query.Where(c => c.Status != CourseStatus.Draft);
        }

        if (userRole == UserRole.Mentor)
        {
            query = query.Where(c => c.MentorId == userId);
        }
        else if (mentorId.HasValue)
        {
            query = query.Where(c => c.MentorId == mentorId);
        }

        var courseSummaries = await courseRepository.ToPaginatedListAsync(
            query.Select(t => t.ToCourseSummaryResponse()),
            pageSize,
            pageIndex);

        return Result.Success(courseSummaries, OK);
    }

    public async Task<Result<CourseSummaryResponse>> GetByIdAsync(Guid id)
    {
        var course = await courseRepository.GetByIdAsync(id);
        if (course == null)
        {
            return Result.Failure<CourseSummaryResponse>("Course not found", NotFound);
        }

        var response = course.ToCourseSummaryResponse();
        return Result.Success(response, OK);
    }

    public async Task<Result<CourseSummaryResponse>> CreateAsync(Guid mentorId, CourseCreateRequest request)
    {
        // RESOLVED: Work item 143#17488893
        var courseWithSameTitle = await courseRepository.GetByTitleAsync(request.Title);

        if (courseWithSameTitle?.CategoryId == request.CategoryId)
        {
            return Result.Failure<CourseSummaryResponse>(
                "Already have this course",
                Conflict);
        }

        // RESOLVED: Work item 250#17489379
        var category = await categoryRepository.GetByIdAsync(request.CategoryId);
        if (category == null)
        {
            return Result.Failure<CourseSummaryResponse>(
                "Category not found.",
                BadRequest);
        }

        if (category.Status == false || category.IsDeleted)
        {
            return Result.Failure<CourseSummaryResponse>(
                $"Category is not active.",
                BadRequest);
        }

        return await CreateAsyncInternal(mentorId, request);
    }

    public async Task<Result<CourseSummaryResponse>> UpdateAsync(Guid id, CourseUpdateRequest request)
    {
        // RESOLVED: Work item 143#17488893
        var course = await courseRepository.GetByIdAsync(id);
        if (course == null)
        {
            return Result.Failure<CourseSummaryResponse>(
                "Course not found",
                NotFound);
        }

        if (request.Title != course.Title)
        {
            var courseWithSameTitle = await courseRepository.GetByTitleAsync(request.Title);
            if (courseWithSameTitle?.CategoryId == request.CategoryId)
            {
                return Result.Failure<CourseSummaryResponse>(
                    "Already have this course",
                    Conflict);
            }
        }

        // RESOLVED: Work item 250#17489379
        var category = await categoryRepository.GetByIdAsync(request.CategoryId);
        if (category == null)
        {
            return Result.Failure<CourseSummaryResponse>(
                "Category not found.",
                BadRequest);
        }

        if (category.Status == false || category.IsDeleted)
        {
            return Result.Failure<CourseSummaryResponse>(
                $"Category {category.Id} is reserved.",
                BadRequest);
        }

        return await UpdateAsyncInternal(course, request);
    }

    public async Task<Result<bool>> DeleteAsync(Guid id)
    {
        var course = await courseRepository.GetByIdAsync(id);
        if (course == null)
        {
            return Result.Failure<bool>("Course not found", NotFound);
        }

        courseRepository.Delete(course);
        await courseRepository.SaveChangesAsync();

        return Result.Success(true, OK);
    }

    public async Task<Result<CourseSummaryResponse>> PublishCourseAsync(Guid id)
    {
        var course = await courseRepository.GetByIdAsync(id);
        if (course == null)
        {
            return Result.Failure<CourseSummaryResponse>("Course not found", NotFound);
        }

        if (course.Status == CourseStatus.Published)
        {
            return Result.Failure<CourseSummaryResponse>("Course is already published", BadRequest);
        }

        course.Status = CourseStatus.Published;
        await courseRepository.SaveChangesAsync();

        var response = course.ToCourseSummaryResponse();
        return Result.Success(response, OK);
    }

    public async Task<Result<CourseSummaryResponse>> ArchiveCourseAsync(Guid id)
    {
        var course = await courseRepository.GetByIdAsync(id);
        if (course == null)
        {
            return Result.Failure<CourseSummaryResponse>("Course not found", NotFound);
        }

        if (course.Status == CourseStatus.Archived)
        {
            return Result.Failure<CourseSummaryResponse>("Course is already archived", BadRequest);
        }

        course.Status = CourseStatus.Archived;
        await courseRepository.SaveChangesAsync();

        var response = course.ToCourseSummaryResponse();
        return Result.Success(response, OK);
    }

    private async Task<Result<CourseSummaryResponse>> CreateAsyncInternal(Guid mentorId, CourseCreateRequest request)
    {
        var caseSensitiveTagNames = request.Tags.ToHashSet();
        var tags = await tagRepository.UpsertAsync(caseSensitiveTagNames);
        await tagRepository.SaveChangesAsync();

        var course = new Course
        {
            Title = request.Title,
            Description = request.Description,
            DueDate = request.DueDate,
            Status = CourseStatus.Draft,
            Difficulty = request.Difficulty,
            CategoryId = request.CategoryId,
            MentorId = mentorId
        };

        await courseRepository.AddAsync(course);
        await courseRepository.UpdateTagsCollection(tags, course);
        await courseRepository.SaveChangesAsync();

        await courseRepository.LoadReferencedEntities(course);
        var response = course.ToCourseSummaryResponse();
        return Result.Success(response, Created);
    }

    private async Task<Result<CourseSummaryResponse>> UpdateAsyncInternal(Course course, CourseUpdateRequest request)
    {
        var caseSensitiveTagNames = request.Tags.ToHashSet();
        var tags = await tagRepository.UpsertAsync(caseSensitiveTagNames);
        await tagRepository.SaveChangesAsync();

        course.Title = request.Title;
        course.Description = request.Description;
        course.CategoryId = request.CategoryId;
        course.DueDate = request.DueDate;
        course.Difficulty = request.Difficulty;

        await courseRepository.UpdateTagsCollection(tags, course);
        await courseRepository.SaveChangesAsync();

        await courseRepository.LoadReferencedEntities(course);
        var response = course.ToCourseSummaryResponse();
        return Result.Success(response, OK);
    }
}