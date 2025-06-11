using Contract.Repositories;
using Domain.Entities;
using Infrastructure.Persistence.Data;
using Infrastructure.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class ConversationRepository(ApplicationDbContext context) : BaseRepository<Conversation, Guid>(context), IConversationRepository
{
    public IQueryable<Conversation> GetAllInclude()
    {
        return GetAll()
            .Include(conv => conv.Participants)
            .ThenInclude(p => p.User)
            .ThenInclude(u => u.Role)
            .Include(p => p.Messages)
            .ThenInclude(s => s.Sender);
    }
}