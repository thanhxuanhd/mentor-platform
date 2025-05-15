using Contract.Dtos.Users.Paginations;
using Contract.Shared;
using Domain.Entities;

namespace Contract.Repositories;

public interface IUserRepository : IBaseRepository<User, Guid>
{
    Task<User?> GetUserByUsername(string fullName);
}   