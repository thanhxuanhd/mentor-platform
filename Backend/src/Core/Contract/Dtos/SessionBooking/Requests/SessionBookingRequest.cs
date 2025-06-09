
using Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Contract.Dtos.SessionBooking.Requests;

public class SessionBookingRequest
{
    public SessionStatus Status { get; set; }
}
public class SessionUpdateRecheduleRequest
{
    public Guid TimeSlotId { get; init; }
    public string? Reason { get; set; }
}
