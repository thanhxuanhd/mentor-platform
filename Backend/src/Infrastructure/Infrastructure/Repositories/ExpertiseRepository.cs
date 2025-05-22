using Contract.Repositories;
using Domain.Entities;
using Infrastructure.Persistence.Data;
using Infrastructure.Repositories.Base;

namespace Infrastructure.Repositories;

public class ExpertiseRepository(ApplicationDbContext context) : BaseRepository<Expertise, Guid>(context), IExpertiseRepository
{
    
}