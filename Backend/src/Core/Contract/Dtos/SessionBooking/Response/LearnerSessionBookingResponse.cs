namespace Contract.Dtos.SessionBooking.Response;

public class LearnerSessionBookingResponse
{
    public Guid Id { get; init; }
    public Guid TimeSlotId { get; init; }
    public Guid LearnerId { get; init; }
    public string? Status { get; init; }
    public int Type { get; init; }
    public TimeOnly StartTime { get; init; }
    public TimeOnly EndTime { get; init; }
    public DateOnly Date { get; init; }
    public string? FullNameLearner { get; init; }
    public string? PreferredCommunicationMethod { get; init; }
}
