
using Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Contract.Dtos.SessionBooking.Requests;

public class SessionBookingRequest
{
    public SessionStatus Status { get; set; }
}
public class SessionUpdateRecheduleRequest
{
    public DateOnly Date { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public string? Reason { get; set; }
}
