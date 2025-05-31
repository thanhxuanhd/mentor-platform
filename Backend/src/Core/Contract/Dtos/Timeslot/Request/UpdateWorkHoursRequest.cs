

namespace Contract.Dtos.Timeslot.Request
{
    public class UpdateWorkHoursRequest
    {
        public Guid MentorId { get; set; }
        public DayOfWeek DayOfWeek { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
    }
}
