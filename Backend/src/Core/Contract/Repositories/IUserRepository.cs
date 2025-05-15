using Contract.Dtos.Users.Paginations;
using Contract.Shared;
using Domain.Entities;

namespace Contract.Repositories;

public interface IUserRepository : IBaseRepository<User, Guid>
{
    Task<User?> GetUserByUsername(string requestUsername);
    Task<List<User>> GetAllUsersWithRole();
    Task<PaginatedList<User>> FilterUser(UserFilterPagedRequest request);
    Task<bool> ExistByEmailExcludeAsync(Guid id, string email);
}