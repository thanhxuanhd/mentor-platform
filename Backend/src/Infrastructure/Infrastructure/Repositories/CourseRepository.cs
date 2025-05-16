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
    public async Task<Course?> GetCourseWithDetailsAsync(Guid id)
    {
        return await _context.Courses
            .Include(c => c.Category)
            .Include(c => c.CourseTags)
            .ThenInclude(ct => ct.Tag)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<PaginatedList<Course>> GetPaginatedCoursesAsync(
        int pageIndex,
        int pageSize,
        Guid? categoryId = null,
        Guid? mentorId = null,
        string? keyword = null,
        CourseState? status = null)
    {
        var query = _context.Courses
            .Include(c => c.Category)
            .Include(c => c.CourseTags)
            .ThenInclude(ct => ct.Tag)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(keyword))
            query = query.Where(c => c.Title.Contains(keyword) || c.Description.Contains(keyword));

        if (categoryId.HasValue) query = query.Where(c => c.CategoryId == categoryId);

        if (mentorId.HasValue) query = query.Where(c => c.MentorId == mentorId);

        if (status.HasValue) query = query.Where(c => c.State == status);

        return await ToPaginatedListAsync(query, pageSize, pageIndex);
    }
}