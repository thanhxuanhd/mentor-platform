using Contract.Repositories;
using Domain.Entities;
using Infrastructure.Persistence.Data;
using Infrastructure.Repositories.Base;

namespace Infrastructure.Repositories;

public class MessageRepository(ApplicationDbContext context) : BaseRepository<Message, Guid>(context), IMessageRepository
{
}