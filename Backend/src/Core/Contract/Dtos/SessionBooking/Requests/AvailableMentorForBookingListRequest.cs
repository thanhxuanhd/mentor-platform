namespace Contract.Dtos.SessionBooking.Requests;

public record AvailableMentorForBookingListRequest
{
    public int PageIndex { get; init; }
    public int PageSize { get; init; }
}