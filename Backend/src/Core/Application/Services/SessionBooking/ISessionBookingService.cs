using Contract.Dtos.SessionBooking.Requests;
using Contract.Dtos.SessionBooking.Response;
using Contract.Shared;

namespace Application.Services.SessionBooking;

public interface ISessionBookingService
{
    Task<Result<PaginatedList<AvailableMentorForBookingResponse>>> GetAllAvailableMentorsAsync(
        AvailableMentorForBookingListRequest request);

    Task<Result<SessionSlotStatusResponse>> RequestBookingAsync(CreateSessionBookingRequest request,
        Guid requestingLearnerId);

    Task<Result<SessionSlotStatusResponse>> AcceptBookingAsync(Guid bookingSessionId, Guid acceptingLearnerId);
    Task<Result<SessionSlotStatusResponse>> CancelBookingAsync(Guid bookingSessionId, Guid cancellingLearnerId);
}