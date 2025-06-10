using Domain.Entities;

namespace Contract.Repositories;

public interface IConversationRepository : IBaseRepository<Conversation, Guid>
{
    IQueryable<Conversation> GetAllInclude();
}