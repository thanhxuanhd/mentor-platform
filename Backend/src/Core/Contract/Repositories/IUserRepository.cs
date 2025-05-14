using Domain.Entities;

namespace Contract.Repositories;

public interface IUserRepository : IBaseRepository<User, Guid>
{
    Task<User?> GetUserByUsername(string requestUsername);
}