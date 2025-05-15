using Contract.Repositories;
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
        var template = _context.Set<User>().AsQueryable();

        if (includeExpressions is not null)
        {
            template = template.Include(includeExpressions);
        }

        return await template.FirstOrDefaultAsync(e => EF.Property<string>(e, "Email") == email);
    }

}