using Domain.Entities;

namespace Contract.Repositories;

public interface IBookRepository : IBaseRepository<Booking, Guid>
{
}
