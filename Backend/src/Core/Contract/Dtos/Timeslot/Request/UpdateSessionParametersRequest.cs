
namespace Contract.Dtos.Timeslot.Request
{
    public class UpdateSessionParametersRequest
    {
        public Guid MentorId { get; set; }
        public int SessionDuration { get; set; } 
        public int BufferTime { get; set; }
        public int SessionDurationMinutes { get; set; }
        public int BufferTimeMinutes { get; set; }
    }
}
