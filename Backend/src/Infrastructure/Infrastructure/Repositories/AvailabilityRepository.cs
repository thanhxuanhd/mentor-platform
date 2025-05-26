using Contract.Repositories;
using Domain.Entities;
using Infrastructure.Persistence.Data;
using Infrastructure.Repositories.Base;

namespace Infrastructure.Repositories;

public class AvailabilityRepository(ApplicationDbContext context) : BaseRepository<Availability, Guid>(context), IAvailabilityRepository
{
    
}