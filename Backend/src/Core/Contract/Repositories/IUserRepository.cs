using Domain.Entities;
using System.Linq.Expressions;

namespace Contract.Repositories;

public interface IUserRepository : IBaseRepository<User, Guid>
{
    Task<User?> GetUserByFullname(string fullname);
    Task<User?> GetUserByEmail(string email);
    Task<User?> GetByEmailAsync(string email, Expression<Func<User, object>>? includeExpressions = null);
    Task<bool> ExistByEmailExcludeAsync(Guid id, string email);
}