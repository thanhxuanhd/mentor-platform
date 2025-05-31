using Domain.Enums;

namespace Contract.Dtos.Schedule.Extensions;

public class TimeSlotResponse
{
    public Guid Id { get; set; }
    public string StartTime { get; set; } = string.Empty;
    public string EndTime { get; set; } = string.Empty;
    public bool IsAvailable { get; set; }
    public bool IsBooked { get; set; }
}

public class TimeSlotRequest 
{
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
}

public class TimeSlotData
{
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public DateOnly Date { get; set; }
    public SessionStatus Status { get; set; }
}