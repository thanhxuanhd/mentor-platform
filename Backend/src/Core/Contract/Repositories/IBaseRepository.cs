using Domain.Abstractions;
using System.Linq.Expressions;

namespace Contract.Repositories;

public interface IBaseRepository<TEntity> where TEntity : BaseEntity
{
    IQueryable<TEntity> GetAll();
    Task<TEntity?> GetByIdAsync(uint id, Expression<Func<TEntity, object>>? includeExpressions = null);
    Task AddAsync(TEntity entity);
    void Update(TEntity entity);
    void Delete(TEntity entity);
    Task AddRangeAsync(List<TEntity> entities);
    Task<int> SaveChangesAsync();
    Task<List<TResponse>> ToListAsync<TResponse>(IQueryable<TResponse> queryable);
}