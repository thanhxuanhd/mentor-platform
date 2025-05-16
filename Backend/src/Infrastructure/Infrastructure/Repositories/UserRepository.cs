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