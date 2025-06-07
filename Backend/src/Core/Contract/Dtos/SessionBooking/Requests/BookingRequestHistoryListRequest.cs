namespace Contract.Dtos.SessionBooking.Requests;

public record BookingRequestHistoryListRequest
{
    public int PageIndex { get; init; }
    public int PageSize { get; init; }
}