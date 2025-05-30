namespace Contract.Dtos.Schedule.Responses;

public class GetScheduleSettingsResponse
{
    public DateOnly WeekStartDate { get; set; }
    public DateOnly WeekEndDate { get; set; }
    public string StartTime { get; set; } = string.Empty; 
    public string EndTime { get; set; } = string.Empty; 
    public int SessionDuration { get; set; } 
    public int BufferTime { get; set; }      
    public bool IsLocked { get; set; }
}

