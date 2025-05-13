using Contract.Repositories;
using Domain.Entities;
using Infrastructure.Persistence.Data;
using Infrastructure.Persistence.Settings;
using Infrastructure.Repositories.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Infrastructure.Repositories;

public class UserRepository(ApplicationDbContext context, IOptions<PaginationSetting> paginationOptions) : BaseRepository<User, Guid>(context, paginationOptions), IUserRepository
{
    public async Task<User?> GetUserByUsername(string username)
    {
        var user = await _context.Users
            .Include(user => user.Role)
            .FirstOrDefaultAsync(u => u.Username.Equals(username));

        return user;
    }
}