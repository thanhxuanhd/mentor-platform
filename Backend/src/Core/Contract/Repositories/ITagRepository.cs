using Domain.Entities;

namespace Contract.Repositories;

public interface ITagRepository : IBaseRepository<Tag, Guid>
{
    Task<List<Tag>> UpsertAsync(HashSet<string> tagNames);
}