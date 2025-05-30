namespace Contract.Dtos.Schedule.Requests;

public class CreateScheduleSettings
{
    public Guid UserId { get; set; }
    public DateOnly WeekStartDate { get; set; }
    public DateOnly WeekEndDate { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public int SessionDuration { get; set; }
    public int BufferTime { get; set; }
}