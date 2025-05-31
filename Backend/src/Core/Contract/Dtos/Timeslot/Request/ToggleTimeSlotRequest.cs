using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contract.Dtos.Timeslot.Request
{
    internal class ToggleTimeSlotRequest
    {
        public Guid MentorId { get; set; }
        public DateOnly Date { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }   
        public bool IsSelected { get; set; }
    }
}
