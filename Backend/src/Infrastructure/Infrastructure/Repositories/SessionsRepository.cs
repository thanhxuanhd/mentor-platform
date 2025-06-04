using Contract.Repositories;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Persistence.Data;
using Infrastructure.Repositories.Base;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Infrastructure.Repositories;

public class SessionsRepository(ApplicationDbContext context)
    : BaseRepository<Sessions, Guid>(context), ISessionsRepository
{
    public new IQueryable<Sessions> GetAll()
    {
        return _context.Sessions
            .Include(s => s.TimeSlot)
            .ThenInclude(mats => mats.Schedules)
            .AsSplitQuery();
    }

    public async Task<Sessions?> GetByIdAsync(Guid id)
    {
        return await _context.Sessions
            .Include(s => s.TimeSlot)
            .ThenInclude(mats => mats.Schedules)
            .ThenInclude(s => s.Mentor)
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public IQueryable<Sessions> GetSessionsByLearnerId(Guid learnerId)
    {
        return _context.Sessions
            .Include(s => s.TimeSlot)
                .ThenInclude(mats => mats.Schedules)
                    .ThenInclude(s => s.Mentor)
                        .ThenInclude(m => m.UserExpertises)
                            .ThenInclude(ue => ue.Expertise)
            .Where(s => s.LearnerId == learnerId);
    }
    public void MentorAcceptBookingSession(Sessions bookingSession, Guid learnerId)
    {
        var isBooked = bookingSession.TimeSlot.Sessions.Any(s => s.Status is SessionStatus.Approved or SessionStatus.Completed);
        if (isBooked)
        {
            throw new Exception("Cannot accept this booking session.");
        }
        
        bookingSession.Status = SessionStatus.Approved;
    }

    public void MentorCancelBookingSession(Sessions bookingSession, Guid learnerId)
    {
        var timeSlot = bookingSession.TimeSlot;
        if (bookingSession.Status is not SessionStatus.Pending)
        {
            throw new Exception("Cannot reject this booking session.");
        }

        var isBooked = timeSlot.Sessions.Any(s => s.Status is SessionStatus.Approved or SessionStatus.Completed);
        if (isBooked)
        {
            throw new Exception("Cannot reject this booking session.");
        }

        bookingSession.Status = SessionStatus.Cancelled;
    }
}