using Contract.Dtos.Categories.Requests;
using Contract.Dtos.SessionBooking.Requests;
using Contract.Dtos.SessionBooking.Response;
using Contract.Dtos.Users.Requests;
using Contract.Dtos.Users.Responses;
using Contract.Shared;

namespace Application.Services.SessionBooking;

public interface ISessionBookingService
{
    Task<Result<PaginatedList<AvailableTimeSlotResponse>>> GetAllAvailableTimeSlotAsync(
        AvailableTimeSlotListRequest request);

    Task<Result<PaginatedList<AvailableMentorForBookingResponse>>> GetAllAvailableMentorForBookingAsync(
        AvailableMentorForBookingListRequest request);
    
    Task<Result<PaginatedList<AvailableTimeSlotResponse>>> GetAllAvailableTimeSlotByMentorAsync(Guid mentorId,
        AvailableTimeSlotListRequest request);
    
    Task<Result<List<AvailableTimeSlotResponse>>> GetAllAvailableTimeSlotByMentorAndDateAsync(Guid mentorId,
        AvailableTimeSlotByDateListRequest request);
    
    Task<Result<List<SessionSlotStatusResponse>>> GetAllBookingRequestByTimeSlot(Guid timeSlotId);

    Task<Result<SessionSlotStatusResponse>> RequestBookingAsync(CreateSessionBookingRequest request,
        Guid requestingLearnerId);

    Task<Result<SessionSlotStatusResponse>> AcceptBookingAsync(Guid bookingSessionId, Guid acceptingLearnerId);
    Task<Result<SessionSlotStatusResponse>> CancelBookingAsync(Guid bookingSessionId, Guid cancellingLearnerId);

    Task<Result<List<LearnerSessionBookingResponse>>> GetAllBooking();
    Task<Result<LearnerSessionBookingResponse>> GetSessionsBookingByIdAsync(Guid id);
    Task<Result<bool>> UpdateStatusSessionAsync(Guid id, SessionBookingRequest request);
    Task<Result<bool>> UpdateRecheduleSessionAsync(Guid id, SessionUpdateRecheduleRequest request);
}