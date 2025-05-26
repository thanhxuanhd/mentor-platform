using Domain.Entities;

namespace Contract.Repositories;

public interface IMentorApplicationRepository : IBaseRepository<MentorApplication, Guid>
{
    public IQueryable<MentorApplication> GetAllApplicationsAsync();
    Task<MentorApplication?> GetMentorApplicationByIdAsync(Guid applicationId);
}
