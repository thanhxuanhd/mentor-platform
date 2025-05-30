namespace Contract.Dtos.Schedule.Extensions;

public class TimeSlotResponse
{
    public Guid Id { get; set; }
    public string StartTime { get; set; }
    public string EndTime { get; set; }
    public bool IsAvailable { get; set; }
    public bool IsBooked { get; set; }
}

public class TimeSlotRequest 
{
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
}
