namespace Contract.Dtos.SessionBooking.Requests;

public record AvailableTimeSlotListRequest
{
    public int PageIndex { get; init; }
    public int PageSize { get; init; }
}