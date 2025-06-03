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
            .Include(ma => ma.Mentor)
                .ThenInclude(m => m.UserExpertises)
                    .ThenInclude(ue => ue.Expertise);
    }

    public async Task<MentorApplication?> GetMentorApplicationByIdAsync(Guid applicationId)
    {
        return await _context.MentorApplications
            .Include(ma => ma.ApplicationDocuments)
            .Include(ma => ma.Admin)
            .Include(ma => ma.Mentor)
                .ThenInclude(m => m.UserExpertises)
                    .ThenInclude(ue => ue.Expertise)
            .FirstOrDefaultAsync(ma => ma.Id == applicationId);
    }

    public IQueryable<MentorApplication> GetMentorApplicationByMentorIdAsync(Guid mentorId)
    {
        return GetAll()
            .Include(ma => ma.Mentor)
            .ThenInclude(m => m.UserExpertises)
            .ThenInclude(ue => ue.Expertise)
            .Where(u => u.MentorId.Equals(mentorId));
    }

    public async Task<MentorApplication?> GetMentorApplicationsToUpdate(Guid applicationId)
    {
        return await GetAll()
            .Include(ma => ma.Mentor)
            .Include(ma => ma.Admin)
            .Include(ma => ma.ApplicationDocuments)
            .FirstOrDefaultAsync(ma => ma.Id == applicationId);
    }
}
