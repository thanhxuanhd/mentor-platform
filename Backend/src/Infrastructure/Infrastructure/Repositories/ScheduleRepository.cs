using Contract.Repositories;
using Domain.Entities;
using Infrastructure.Persistence.Data;
using Infrastructure.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class ScheduleRepository(ApplicationDbContext context) : BaseRepository<Schedules, Guid>(context), IScheduleRepository
{
    public async Task<Schedules?> GetScheduleSettingsAsync(Guid mentorId, DateOnly weekStartDate, DateOnly weekEndDate)
    {
        return await _context.Schedules
            .FirstOrDefaultAsync(s => s.MentorId == mentorId && s.WeekStartDate == weekStartDate && s.WeekEndDate == weekEndDate);
    }
}