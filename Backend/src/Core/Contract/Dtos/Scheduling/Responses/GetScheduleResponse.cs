namespace Contract.Dtos.Scheduling.Responses;

public class GetScheduleResponse
{
    public Guid Id { get; set; } 
    public DayOfWeek DayOfWeek { get; set; } 
    public string StartTime { get; set; } = string.Empty; 
    public string EndTime { get; set; } = string.Empty; 
    public int SessionDuration { get; set; } 
    public int BufferTime { get; set; }      
    public bool IsLocked { get; set; }
}

