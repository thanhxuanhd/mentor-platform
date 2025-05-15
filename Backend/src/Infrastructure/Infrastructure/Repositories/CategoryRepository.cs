using Contract.Repositories;
using Domain.Entities;
using Infrastructure.Persistence.Data;
using Infrastructure.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class CategoryRepository(ApplicationDbContext context) : BaseRepository<Category, Guid>(context), ICategoryRepository
{
    public Task<bool> ExistByNameAsync(string name)
    {
        return _context.Categories
            .AnyAsync(c => c.Name == name);
    }

    public IQueryable<Course> FilterCourseByCategory(Guid id)
    {
        var courses = _context.Courses
            .Where(c => c.CategoryId == id)
            .Include(c => c.Category)
            .Include(c => c.CourseTags)
            .ThenInclude(ct => ct.Tag)
            .AsQueryable();

        return courses;
    }
}
