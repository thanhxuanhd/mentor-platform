using Contract.Repositories;
using Contract.Shared;
using Domain.Abstractions;
using Infrastructure.Persistence.Data;
using Infrastructure.Persistence.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Linq.Expressions;

namespace Infrastructure.Repositories.Base;

public class BaseRepository<TEntity, TPrimaryKey>(ApplicationDbContext context, IOptions<PaginationSetting> paginationOptions) : IBaseRepository<TEntity, TPrimaryKey>
    where TEntity : BaseEntity<TPrimaryKey> where TPrimaryKey : struct
{
    protected readonly ApplicationDbContext _context = context;
    private readonly PaginationSetting _paginationSetting = paginationOptions.Value;

    public IQueryable<TEntity> GetAll()
    {
        return _context.Set<TEntity>().AsQueryable();
    }

    public virtual async Task<TEntity?> GetByIdAsync(TPrimaryKey id, Expression<Func<TEntity, object>>? includeExpressions = null)
    {
        var template = _context.Set<TEntity>().AsQueryable();
        if (includeExpressions is not null)
        {
           template = template.Include(includeExpressions);
        }
        var entity = await template.FirstOrDefaultAsync(e => e.Id.Equals(id));

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

    public async Task<PaginatedList<TResponse>> ToPaginatedListAsync<TResponse>(IQueryable<TResponse> queryable,
        int pageSize = 0, int pageIndex = 0)
    {
        pageSize = pageSize > 0 ? pageSize : _paginationSetting.DefaultPageSize;
        pageIndex = pageIndex > 0 ? pageIndex : _paginationSetting.DefaultPageIndex;
        var count = await queryable.CountAsync();
        var paginatedResponse = await queryable.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();

        return new PaginatedList<TResponse>(paginatedResponse, count, pageIndex, pageSize);
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