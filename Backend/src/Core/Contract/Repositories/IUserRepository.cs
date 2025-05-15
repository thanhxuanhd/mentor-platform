using Contract.Dtos.Users.Paginations;
using Contract.Shared;
using Domain.Entities;
using System.Linq.Expressions;

namespace Contract.Repositories;

public interface IUserRepository : IBaseRepository<User, Guid>
{
    Task<User?> GetUserByEmail(string requestUsername);
    Task<User?> GetByEmailAsync(string email, Expression<Func<User, object>>? includeExpressions = null);

}