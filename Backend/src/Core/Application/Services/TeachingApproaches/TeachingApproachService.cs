using System.Net;
using Contract.Dtos.TeachingApproaches.Responses;
using Contract.Repositories;
using Contract.Shared;

namespace Application.Services.TeachingApproaches;

public class TeachingApproachService(ITeachingApproachRepository teachingApproachRepository) : ITeachingApproachService
{
    public async Task<Result<List<GetTeachingApproachResponse>>> GetAllTeachingApproachesAsync()
    {
        var teachingApproaches = teachingApproachRepository.GetAll().Select(a => new GetTeachingApproachResponse(a.Id, a.Name));
        var result = await teachingApproachRepository.ToListAsync(teachingApproaches);

        return Result.Success(result, HttpStatusCode.OK);
    }
}