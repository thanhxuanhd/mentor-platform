using Contract.Dtos.Expertises.Responses;
using Contract.Shared;

namespace Application.Services.Expertises;

public interface IExpertiseService
{
    Task<Result<List<GetExpertiseResponse>>> GetAllExpertisesAsync();
}