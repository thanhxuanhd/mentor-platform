using Contract.Repositories;
using Domain.Entities;

namespace Application.Services.MentorTimeSlot;

public class TimeSlotService(ITimeSlotRepository timeslotRepository, IScheduleRepository scheduleRepository) : ITimeSlotService
{
    
}
