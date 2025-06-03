namespace Contract.Dtos.SessionBooking.Requests;

public record AvailableTimeSlotByDateListRequest
{
    public DateOnly Date { get; init; }
}