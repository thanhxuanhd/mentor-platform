
namespace Contract.Dtos.Scheduling.Requests
{
    public class ScheduleRequest
    {
        public Guid UserId { get; set; }
        public DayOfWeek DayOfWeek { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public int SessionDuration { get; set; }
        public int BufferTime { get; set; }
        public bool IsLocked { get; set; }
    }
}
