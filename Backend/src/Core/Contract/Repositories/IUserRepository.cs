using Domain.Entities;
using System.Linq.Expressions;
using Domain.Abstractions;

namespace Contract.Repositories;

public interface IUserRepository : IBaseRepository<User, Guid>
{
    Task<User?> GetUserByEmail(string email);
    Task<User?> GetByEmailAsync(string email, Expression<Func<User, object>>? includeExpressions = null);
    Task<bool> ExistByEmailExcludeAsync(Guid id, string email);
    Task<bool> CheckEntityListExist<TEntity, TPrimaryKey>(List<TPrimaryKey> listIds) where TEntity : BaseEntity<TPrimaryKey> where TPrimaryKey : struct;
    Task<User?> GetUserDetailAsync(Guid id);
}