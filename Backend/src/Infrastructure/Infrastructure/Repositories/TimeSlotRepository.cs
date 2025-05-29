using Contract.Repositories;
using Domain.Entities;
using Infrastructure.Persistence.Data;
using Infrastructure.Repositories.Base;

namespace Infrastructure.Repositories;

public class TimeSlotRepository(ApplicationDbContext context) : BaseRepository<MentorAvailableTimeSlot, Guid>(context), ITimeSlotRepository
{

}
