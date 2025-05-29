namespace Contract.Dtos.Timeslot.Response
{
    public class LockedStatusResponse
    {
        public Guid MentorId { get; set; }
        public bool IsLocked { get; set; }
        public DateTime? LockedUntil { get; set; }
    }
}
