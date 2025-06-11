using Contract.Dtos.SessionBooking.Requests;
using Contract.Dtos.SessionBooking.Response;
using Contract.Shared;

namespace Application.Services.SessionBooking;

public interface ISessionBookingService
{
    Task<Result<PaginatedList<AvailableTimeSlotResponse>>> GetAllAvailableTimeSlotAsync(
        AvailableTimeSlotListRequest request);

    Task<Result<List<AvailableMentorForBookingResponse>>> GetAllAvailableMentorForBookingAsync();


    Task<Result<PaginatedList<AvailableTimeSlotResponse>>> GetAllTimeSlotByMentorAsync(Guid mentorId,
        AvailableTimeSlotListRequest request);
    
    Task<Result<List<TimeSlotByMentorAndDateResponse>>> GetAllTimeSlotByMentorAndDateAsync(Guid mentorId,
        Guid learnerId,
        AvailableTimeSlotByDateListRequest request);
    
    Task<Result<List<SessionSlotStatusResponse>>> GetAllBookingRequestByTimeSlot(Guid timeSlotId);
    Task<Result<List<GetAllRequestByLearnerResponse>>> GetAllBookingRequestByLearnerId(Guid learnerId);

    Task<Result<SessionSlotStatusResponse>> RequestBookingAsync(CreateSessionBookingRequest request,
        Guid requestingLearnerId);

    Task<Result<SessionSlotStatusResponse>> AcceptBookingAsync(Guid bookingSessionId, Guid acceptingLearnerId);
    Task<Result<SessionSlotStatusResponse>> CancelBookingAsync(Guid bookingSessionId, Guid cancellingLearnerId);

    Task<Result<List<LearnerSessionBookingResponse>>> GetAllBooking(Guid MentorId);
    Task<Result<LearnerSessionBookingResponse>> GetSessionsBookingByIdAsync(Guid id);
    Task<Result<bool>> UpdateStatusSessionAsync(Guid id, SessionBookingRequest request);
    Task<Result<bool>> UpdateRecheduleSessionAsync(Guid id, SessionUpdateRecheduleRequest request);
    Task<Result<List<AvailableTimeSlotResponse>>> GetAllTimeSlotByMentorAsync(Guid mentorId, DateOnly date);
}