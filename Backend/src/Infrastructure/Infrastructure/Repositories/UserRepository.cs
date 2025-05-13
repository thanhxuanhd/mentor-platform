using Contract.Repositories;
using Domain.Entities;
using Infrastructure.Persistence.Data;
using Infrastructure.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class UserRepository(ApplicationDbContext context) : BaseRepository<User>(context), IUserRepository
{
    public async Task<User?> GetUserByUsername(string username)
    {
        var user = await _context.Users
            .Include(user => user.Role)
            .FirstOrDefaultAsync(u => u.Username.Equals(username));

        return user;
    }
}