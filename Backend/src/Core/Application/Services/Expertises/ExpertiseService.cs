using System.Net;
using Contract.Dtos.Expertises.Responses;
using Contract.Repositories;
using Contract.Shared;

namespace Application.Services.Expertises;

public class ExpertiseService(IExpertiseRepository expertiseRepository) : IExpertiseService
{
    public async Task<Result<List<GetExpertiseResponse>>> GetAllExpertisesAsync()
    {
        var expertises = expertiseRepository.GetAll().Select(a => new GetExpertiseResponse(a.Id, a.Name));
        var result = await expertiseRepository.ToListAsync(expertises);

        return Result.Success(result, HttpStatusCode.OK);
    }
}