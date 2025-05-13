using Domain.Abstractions;
using System.Linq.Expressions;
using Contract.Shared;
using Contract.Shared.Constants;

namespace Contract.Repositories;

public interface IBaseRepository<TEntity, TPrimaryKey> where TEntity : BaseEntity<TPrimaryKey> where TPrimaryKey : struct
{
    IQueryable<TEntity> GetAll();
    Task<TEntity?> GetByIdAsync(TPrimaryKey id, Expression<Func<TEntity, object>>? includeExpressions = null);
    Task AddAsync(TEntity entity);
    void Update(TEntity entity);
    void Delete(TEntity entity);
    Task AddRangeAsync(List<TEntity> entities);
    Task<int> SaveChangesAsync();
    Task<List<TResponse>> ToListAsync<TResponse>(IQueryable<TResponse> queryable);

    Task<PaginatedList<TResponse>> ToPaginatedListAsync<TResponse>(IQueryable<TResponse> queryable,
        int pageSize = PaginationConstant.DefaultPageSize, int pageIndex = PaginationConstant.DefaultPageIndex);
}