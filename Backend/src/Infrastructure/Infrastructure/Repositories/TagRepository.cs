using Contract.Repositories;
using Domain.Entities;
using Infrastructure.Persistence.Data;
using Infrastructure.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class TagRepository(ApplicationDbContext context) : BaseRepository<Tag, Guid>(context), ITagRepository
{
    public async Task<List<Tag>> UpsertAsync(HashSet<string> tagNames)
    {
        var tags = await _context.Tags.Where(t => tagNames.Contains(t.Name)).ToListAsync();
        var existingTagNames = tags.Select(t => t.Name).ToHashSet();

        foreach (var tagName in tagNames)
            if (!existingTagNames.Contains(tagName))
            {
                var tag = new Tag { Name = tagName };
                _context.Tags.Add(tag);
                tags.Add(tag);
            }

        return tags;
    }
}