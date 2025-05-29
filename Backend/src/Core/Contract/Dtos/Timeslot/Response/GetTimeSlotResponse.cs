using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contract.Dtos.Timeslot.Response;

public class GetTimeSlotResponse
{
    public Guid MentorId { get; set; }
    public Guid ScheduleId { get; set; }
    public Guid SessionId { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public virtual User? Mentor { get; set; }
    public virtual ICollection<Booking>? Bookings { get; set; }
}
