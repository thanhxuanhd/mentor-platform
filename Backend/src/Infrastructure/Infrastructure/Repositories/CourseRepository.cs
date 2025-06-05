using Contract.Repositories;
using Domain.Entities;
using Infrastructure.Persistence.Data;
using Infrastructure.Repositories.Base;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Infrastructure.Repositories;

public class CourseRepository(ApplicationDbContext context) : BaseRepository<Course, Guid>(context), ICourseRepository
{
    public new IQueryable<Course> GetAll()
    {
        var query = _context.Courses
            .OrderBy(c => c.Id)
            .Include(c => c.Category)
            .Include(c => c.Mentor)
            .Include(c => c.Tags)
            .Include(c => c.Resources)
            .AsSplitQuery()
            .AsQueryable();
        return query;
    }

    public new async Task<Course?> GetByIdAsync(Guid id,
        Expression<Func<Course, object>>? includeExpressions = null)
    {
        return await _context.Courses
            .Include(c => c.Category)
            .Include(c => c.Mentor)
            .Include(c => c.Tags)
            .Include(c => c.Resources)
            .AsSplitQuery()
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<Course?> GetByTitleAsync(string title)
    {
        return await _context.Courses
            .Include(c => c.Category)
            .Include(c => c.Mentor)
            .AsSplitQuery()
            .FirstOrDefaultAsync(c => c.Title == title);
    }

    public async Task UpdateTagsCollection(List<Tag> tags, Course course)
    {
        await _context.Entry(course).Collection(c => c.Tags).LoadAsync();
        course.Tags.Clear();
        course.Tags.AddRange(tags);
    }

    public async Task LoadReferencedEntities(Course course)
    {
        await _context.Entry(course).Reference(c => c.Category).LoadAsync();
        await _context.Entry(course).Reference(c => c.Mentor).LoadAsync();
    }
}