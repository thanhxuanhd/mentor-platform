using Contract.Dtos.TeachingApproaches.Responses;
using Contract.Shared;

namespace Application.Services.TeachingApproaches;

public interface ITeachingApproachService
{
    Task<Result<List<GetTeachingApproachResponse>>> GetAllTeachingApproachesAsync();
}