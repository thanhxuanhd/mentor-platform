using System.Linq.Expressions;
using Contract.Repositories;
using Domain.Abstractions;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Persistence.Data;
using Infrastructure.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class UserRepository(ApplicationDbContext context) : BaseRepository<User, Guid>(context), IUserRepository
{
    public async Task<User?> GetUserByEmail(string email)
    {
        var user = await _context.Users
            .Include(user => user.Role)
            .Where(u => !u.Status.Equals(UserStatus.Deactivated))
            .FirstOrDefaultAsync(u => u.Email.Equals(email));

        return user;
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

    public Task<bool> ExistByEmailExcludeAsync(Guid id, string email)
    {
        return _context.Users
            .AnyAsync(e => e.Email == email && e.Id != id);
    }

    public async Task<User?> GetUserDetailAsync(Guid id)
    {
        var user = await _context.Users
            .Include(u => u.Role)
            .Include(u => u.UserCategories)
            .Include(u => u.UserAvailabilities)
            .Include(u => u.UserExpertises)
            .ThenInclude(u => u.Expertise)
            .Include(u => u.UserTeachingApproaches)
            .Where(u => !u.Status.Equals(UserStatus.Deactivated))
            .FirstOrDefaultAsync(u => u.Id.Equals(id));

        return user;
    }

    public async Task<bool> CheckEntityListExist<TEntity, TPrimaryKey>(List<TPrimaryKey> listIds) where TEntity : BaseEntity<TPrimaryKey> where TPrimaryKey : struct
    {
        var validExpertiseIds = await _context.Set<TEntity>()
            .Where(e => listIds.Contains(e.Id))
            .Select(e => e.Id)
            .ToListAsync();

        return validExpertiseIds.Count == listIds.Count;
    }
}