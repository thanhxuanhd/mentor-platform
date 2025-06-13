using Domain.Entities;

namespace Contract.Repositories;

public interface IConversationParticipantRepository : IBaseRepository<ConversationParticipant, Guid>
{
}