using Contract.Dtos.Courses.Responses;
using Contract.Repositories;
using Contract.Shared;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Persistence.Data;
using Infrastructure.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class CourseRepository(ApplicationDbContext context) : BaseRepository<Course, Guid>(context), ICourseRepository
{
    public async Task UpdateTagsCollection(List<Tag> tags, Course course)
    {
        var tagIds = tags.Select(t => t.Id);

        await _context.CourseTags
            .Where(ct => ct.CourseId == course.Id && !tagIds.Contains(ct.TagId))
            .ExecuteDeleteAsync();

        foreach (var courseTag in tagIds.Select(tagId => new CourseTag
                 {
                     CourseId = course.Id,
                     TagId = tagId
                 }))
        {
            var trackedCourseTag = await _context.CourseTags.FindAsync(course.Id, courseTag.TagId);
            if (trackedCourseTag == null)
            {
                _context.Attach(courseTag);
                _context.Entry(courseTag).State = EntityState.Added;
            }
        }
    }

    public async Task<Course?> GetCourseWithDetailsAsync(Guid id)
    {
        return await _context.Courses
            .Include(c => c.Category)
            .Include(c => c.Mentor)
            .Include(c => c.CourseTags)
            .ThenInclude(c => c.Tag)
            // .Include(c => c.Tags)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<PaginatedList<CourseSummary>> GetPaginatedCoursesAsync(
        int pageIndex,
        int pageSize,
        Guid? categoryId = null,
        Guid? mentorId = null,
        string? keyword = null,
        CourseStatus? status = null,
        CourseDifficulty? difficulty = null)
    {
        var query = _context.Courses
            .OrderBy(c => c.Id)
            .Include(c => c.Category)
            .Include(c => c.Mentor)
            .Include(c => c.Tags)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(keyword))
            query = query.Where(c => c.Title.Contains(keyword) || c.Description.Contains(keyword));

        if (categoryId.HasValue) query = query.Where(c => c.CategoryId == categoryId);

        if (mentorId.HasValue) query = query.Where(c => c.MentorId == mentorId);

        if (status.HasValue) query = query.Where(c => c.Status == status);

        if (difficulty.HasValue) query = query.Where(c => c.Difficulty == difficulty);

        var courseSummaries = await query.Select(c => c.ToCourseSummary()).ToListAsync();

        return new PaginatedList<CourseSummary>(courseSummaries, courseSummaries.Count, pageIndex, pageSize);
    }
}