using Domain.Entities;
using System.Linq.Expressions;

namespace Contract.Repositories;

public interface IUserRepository : IBaseRepository<User, Guid>
{
    Task<User?> GetUserByEmail(string requestUsername);
}