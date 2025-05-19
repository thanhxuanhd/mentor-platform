using Contract.Repositories;
using Domain.Entities;
using Infrastructure.Persistence.Data;
using Infrastructure.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class CourseItemRepository(ApplicationDbContext context)
    : BaseRepository<CourseItem, Guid>(context), ICourseItemRepository
{
    public async Task<CourseItem?> GetByIdAsync(Guid courseId, Guid resourceId)
    {
        return await _context.CourseItems.FirstOrDefaultAsync(i => i.CourseId == courseId && i.Id == resourceId);
    }

    public async Task<List<CourseItem>> GetAllByCourseIdAsync(Guid courseId)
    {
        return await _context.CourseItems
            .Where(i => i.CourseId == courseId)
            .OrderBy(i => i.Id)
            .ToListAsync();
    }
}
