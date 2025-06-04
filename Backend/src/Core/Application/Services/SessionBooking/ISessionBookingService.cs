using Contract.Dtos.SessionBooking.Requests;
using Contract.Dtos.SessionBooking.Response;
using Contract.Shared;

namespace Application.Services.SessionBooking;

public interface ISessionBookingService
{
    Task<Result<PaginatedList<AvailableTimeSlotResponse>>> GetAllAvailableTimeSlotAsync(
        AvailableTimeSlotListRequest request);

    Task<Result<List<AvailableMentorForBookingResponse>>> GetAllAvailableMentorForBookingAsync();


    Task<Result<PaginatedList<AvailableTimeSlotResponse>>> GetAllAvailableTimeSlotByMentorAsync(Guid mentorId,
        AvailableTimeSlotListRequest request);
    
    Task<Result<List<AvailableTimeSlotResponse>>> GetAllAvailableTimeSlotByMentorAndDateAsync(Guid mentorId,
        AvailableTimeSlotByDateListRequest request);
    
    Task<Result<List<SessionSlotStatusResponse>>> GetAllBookingRequestByTimeSlot(Guid timeSlotId);
    Task<Result<List<GetAllRequestByLearnerResponse>>> GetAllBookingRequestByLearnerId(Guid learnerId);

    Task<Result<SessionSlotStatusResponse>> RequestBookingAsync(CreateSessionBookingRequest request,
        Guid requestingLearnerId);

    Task<Result<SessionSlotStatusResponse>> AcceptBookingAsync(Guid bookingSessionId, Guid acceptingLearnerId);
    Task<Result<SessionSlotStatusResponse>> CancelBookingAsync(Guid bookingSessionId, Guid cancellingLearnerId);
}