namespace Contract.Dtos.SessionBooking.Response;

public record AvailableMentorForBookingResponse
{
    public required Guid MentorId { get; init; }
    public required string MentorName { get; init; }
    public required List<string> MentorExpertise { get; init; } = [];
    public required string? MentorAvatarUrl { get; init; }
    public required TimeOnly WorkingStartTime { get; init; }
    public required TimeOnly WorkingEndTime { get; init; }
}