using Contract.Repositories;
using Domain.Entities;
using Infrastructure.Persistence.Data;
using Infrastructure.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class MentorApplicationRepository(ApplicationDbContext context) : BaseRepository<MentorApplication, Guid>(context), IMentorApplicationRepository
{
    public IQueryable<MentorApplication> GetAllApplicationsAsync()
    {
        return _context.MentorApplications
            .Include(ma => ma.Mentor)
                .ThenInclude(m => m.UserExpertises);
    }

    public async Task<MentorApplication?> GetMentorApplicationByIdAsync(Guid applicationId)
    {
        return await _context.MentorApplications
            .Include(ma => ma.Mentor)
                .ThenInclude(m => m.UserExpertises)
            .Include(ma => ma.ApplicationDocuments)            
            .FirstOrDefaultAsync(ma => ma.Id == applicationId);
    }

}
