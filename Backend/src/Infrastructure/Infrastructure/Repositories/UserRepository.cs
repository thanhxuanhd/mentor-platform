using Contract.Dtos.Users.Paginations;
using Contract.Repositories;
using Contract.Shared;
using Domain.Entities;
using Infrastructure.Persistence.Data;
using Infrastructure.Repositories.Base;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Infrastructure.Repositories;

public class UserRepository(ApplicationDbContext context) : BaseRepository<User, Guid>(context), IUserRepository
{
    public async Task<User?> GetUserByEmail(string email)
    {
        var user = await _context.Users
            .Include(user => user.Role)
            .FirstOrDefaultAsync(u => u.Email.Equals(email));
        return user;
    }

    public async Task<PaginatedList<User>> FilterUser(UserFilterPagedRequest request)
    {
        var query = GetAll()
            .Include(user => user.Role)
            .AsQueryable();

        if (!string.IsNullOrEmpty(request.RoleName))
        {
            query = query.Where(user => user.Role.Name.ToString().Equals(request.RoleName));
        }

        if (!string.IsNullOrEmpty(request.FullName))
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

    public Task<bool> ExistByEmailExcludeAsync(Guid id, string email)
    {
        return _context.Users
            .AnyAsync(u => u.Email == email && u.Id != id);
    }

    public virtual async Task<User?> GetByEmailAsync(string email, Expression<Func<User, object>>? includeExpressions = null)
    {
        var user = _context.Users.AsQueryable();

        if (includeExpressions is not null)
        {
            user = user.Include(includeExpressions);
        }

        return await user.FirstOrDefaultAsync(e => e.Email == email);
    }

}