using Contract.Repositories;
using Domain.Abstractions;
using Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Infrastructure.Repositories.Base;

public class BaseRepository<TEntity>(ApplicationDbContext context) : IBaseRepository<TEntity>
    where TEntity : BaseEntity
{
    protected readonly ApplicationDbContext _context = context;

    public IQueryable<TEntity> GetAll()
    {
        return _context.Set<TEntity>().AsQueryable();
    }

    public virtual async Task<TEntity?> GetByIdAsync(uint id, Expression<Func<TEntity, object>>? includeExpressions = null)
    {
        var template = _context.Set<TEntity>().AsQueryable();
        if (includeExpressions is not null)
        {
           template = template.Include(includeExpressions);
        }
        var entity = await template.FirstOrDefaultAsync(e => e.Id == id);

        return entity;
    }

    public virtual async Task AddAsync(TEntity entity)
    {
        await _context.Set<TEntity>().AddAsync(entity);
    }

    public virtual async Task AddRangeAsync(List<TEntity> entities)
    {
        await _context.Set<TEntity>().AddRangeAsync(entities);
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public async Task<List<TResponse>> ToListAsync<TResponse>(IQueryable<TResponse> queryable)
    {
        return await queryable.ToListAsync();
    }

    public virtual void Update(TEntity entity)
    {
        _context.Set<TEntity>().Update(entity);
    }

    public virtual void Delete(TEntity entity)
    {
        _context.Set<TEntity>().Remove(entity);
    }

}