using Domain.Entities;

namespace Contract.Repositories;

public interface IMentorApplicationRepository : IBaseRepository<MentorApplication, Guid>
{
    IQueryable<MentorApplication> GetAllApplicationsAsync();
    Task<MentorApplication?> GetMentorApplicationByIdAsync(Guid applicationId);
    IQueryable<MentorApplication> GetMentorApplicationByMentorIdAsync(Guid applicationId);
    Task<MentorApplication?> GetMentorApplicationsToUpdate(Guid applicationId);
}
