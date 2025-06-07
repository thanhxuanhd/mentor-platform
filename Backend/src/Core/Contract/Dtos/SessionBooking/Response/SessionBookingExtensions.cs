using Domain.Entities;
using Domain.Enums;

namespace Contract.Dtos.SessionBooking.Response;

public static class SessionBookingExtensions
{
    public static AvailableTimeSlotResponse ToAvailableTimeSlotResponse(
        this MentorAvailableTimeSlot mats)
    {
        return new AvailableTimeSlotResponse
        {
            MentorId = mats.Schedules.MentorId,
            MentorName = mats.Schedules.Mentor.FullName,
            StartTime = mats.StartTime,
            EndTime = mats.EndTime,
            Date = mats.Date,
            IsBooked = mats.Sessions.Any(s => s.Status is SessionStatus.Approved or SessionStatus.Completed or SessionStatus.Rescheduled)
        };
    }

    public static AvailableMentorForBookingResponse CreateAvailableMentorForBookingResponse(
        User user, Schedules earliestSchedule)
    {
        return new AvailableMentorForBookingResponse
        {
            MentorId = user.Id,
            MentorName = user.FullName,
            MentorExpertise = user.UserExpertises.Select(ue => ue.Expertise!.Name).ToList(),
            MentorAvatarUrl = user.ProfilePhotoUrl,
            WorkingEndTime = earliestSchedule.StartHour,
            WorkingStartTime = earliestSchedule.EndHour
        };
    }

    public static SessionSlotStatusResponse ToSessionSlotStatusResponse(this Sessions booking)
    {
        var mats = booking.TimeSlot;
        return new SessionSlotStatusResponse
        {
            SlotId = booking.Id,
            MentorId = mats.Schedules.MentorId,
            BookingStatus = booking.Status,
            Day = mats.Date,
            StartTime = mats.StartTime,
            EndTime = mats.EndTime
        };
    }
}