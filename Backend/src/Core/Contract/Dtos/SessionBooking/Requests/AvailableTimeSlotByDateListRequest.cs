namespace Contract.Dtos.SessionBooking.Requests;

public record AvailableTimeSlotByDateListRequest
{
    public DateTime Date { get; init; }
}