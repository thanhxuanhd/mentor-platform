using Contract.Dtos.Users.Paginations;
using Contract.Repositories;
using Contract.Shared;
using Domain.Entities;
using Infrastructure.Persistence.Data;
using Infrastructure.Repositories.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace Infrastructure.Repositories;

public class UserRepository(ApplicationDbContext context) : BaseRepository<User, Guid>(context), IUserRepository
{
    public async Task<List<User>> GetAllUsersWithRole()
    {
        var users = await _context.Users
            .Include(user => user.Role)
            .ToListAsync();
        return users;
    }

    public async Task<User?> GetUserByUsername(string fullName)
    {
        var user = await _context.Users
            .Include(user => user.Role)
            .FirstOrDefaultAsync(u => u.FullName.Equals(fullName));

        return user;
    }

    public async Task<PaginatedList<User>> FilterUser(UserFilterPagedRequest request)
    {
        var query = GetAll()
            .Include(user => user.Role)
            .AsQueryable();

        if (!request.RoleName.IsNullOrEmpty())
        {
            query = query.Where(user => user.Role.Name.Equals(request.RoleName));
        }

        if (!request.FullName.IsNullOrEmpty())
        {
            query = query.Where(user => user.FullName.Contains(request.FullName!));
        }

        var totalItems = await query.CountAsync();

        var items = await query
            .Skip((request.PageIndex - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync();

        return new PaginatedList<User>(items, totalItems, request.PageIndex = 1, request.PageSize = 5);
    }
}