namespace Contract.Dtos.SessionBooking.Requests;

public class SessionUpdateRescheduleRequest
{
    public Guid TimeSlotId { get; init; }
    public string? Reason { get; set; }
}
