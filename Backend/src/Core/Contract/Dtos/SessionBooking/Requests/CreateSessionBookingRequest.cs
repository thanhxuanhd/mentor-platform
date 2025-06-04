using Domain.Enums;

namespace Contract.Dtos.SessionBooking.Requests;

public record CreateSessionBookingRequest
{
    public required Guid TimeSlotId { get; init; }
    public required SessionType SessionType { get; init; }
}