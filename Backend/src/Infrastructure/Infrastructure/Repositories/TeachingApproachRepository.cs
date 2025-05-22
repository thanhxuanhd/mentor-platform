using Contract.Repositories;
using Domain.Entities;
using Infrastructure.Persistence.Data;
using Infrastructure.Repositories.Base;

namespace Infrastructure.Repositories;

public class TeachingApproachRepository(ApplicationDbContext context) : BaseRepository<TeachingApproach, Guid>(context), ITeachingApproachRepository
{
    
}