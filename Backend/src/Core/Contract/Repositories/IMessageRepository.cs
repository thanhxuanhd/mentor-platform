using Domain.Entities;

namespace Contract.Repositories;

public interface IMessageRepository : IBaseRepository<Message, Guid>
{
}