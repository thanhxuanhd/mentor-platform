using System.Net;
using Contract.Dtos.Availabilities.Responses;
using Contract.Repositories;
using Contract.Shared;

namespace Application.Services.Availabilities;

public class AvailabilityService(IAvailabilityRepository availabilityRepository) : IAvailabilityService
{
    public async Task<Result<List<GetAvailabilityResponse>>> GetAllAvailabilitiesAsync()
    {
        var availabilities = availabilityRepository.GetAll()
            .Select(a => new GetAvailabilityResponse(a.Id, a.Name));

        var result = await availabilityRepository.ToListAsync(availabilities);

        return Result.Success(result, HttpStatusCode.OK);
    }
}