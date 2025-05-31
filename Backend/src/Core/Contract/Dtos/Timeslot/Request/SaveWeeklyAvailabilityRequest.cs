
namespace Contract.Dtos.Timeslot.Request
{
    public class SaveWeeklyAvailabilityRequest
    {
        public Guid MentorId { get; set; }
        public List<DailyAvailability> Availability { get; set; } = new();
    }

    public class DailyAvailability
    {
        public DateOnly Date { get; set; }
        public List<TimeBlockDto> TimeBlocks { get; set; } = new();
    }

    public class TimeBlockDto
    {
        public Guid? BlockId { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public bool IsSelected { get; set; }
        public bool IsBooked { get; set; }
    }
    public class WorkHoursDto
    {
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public bool IsLocked { get; set; }
    }

    public class SessionParametersDto
    {
        public int SessionDuration { get; set; }
        public int BufferTime { get; set; }
        public bool IsLocked { get; set; }
    }

}
