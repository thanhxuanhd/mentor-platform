using Contract.Repositories;
using Domain.Entities;
using Infrastructure.Persistence.Data;
using Infrastructure.Repositories.Base;

namespace Infrastructure.Repositories;

public class BookingRepository(ApplicationDbContext context) : BaseRepository<Booking, Guid>(context), IBookRepository
{
}
