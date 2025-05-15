using Contract.Repositories;
using Contract.Shared;
using Domain.Entities;
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

    public async Task<PaginatedList<Course>> GetPaginatedCoursesAsync(int pageIndex, int pageSize, Guid? categoryId = null, Guid? mentorId = null)
    {
        var query = _context.Courses
            .Include(c => c.Category)
            .Include(c => c.CourseTags)
                .ThenInclude(ct => ct.Tag)
            .AsQueryable();

        if (categoryId.HasValue)
        {
            query = query.Where(c => c.CategoryId == categoryId);
        }

        if (mentorId.HasValue)
        {
            query = query.Where(c => c.MentorId == mentorId);
        }

        return await ToPaginatedListAsync(query, pageSize, pageIndex);
    }
}
