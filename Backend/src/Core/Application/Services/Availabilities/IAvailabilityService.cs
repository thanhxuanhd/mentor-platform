using Contract.Dtos.Availabilities.Responses;
using Contract.Shared;

namespace Application.Services.Availabilities;

public interface IAvailabilityService
{
    Task<Result<List<GetAvailabilityResponse>>> GetAllAvailabilitiesAsync();
}