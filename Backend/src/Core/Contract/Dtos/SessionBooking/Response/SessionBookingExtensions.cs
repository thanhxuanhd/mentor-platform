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
            Id = mats.Id,
            MentorId = mats.Schedules.MentorId,
            MentorName = mats.Schedules.Mentor.FullName,
            StartTime = mats.StartTime,
            EndTime = mats.EndTime,
            Date = mats.Date,
            IsBooked = mats.Sessions.Any(s => s.Status is SessionStatus.Approved or SessionStatus.Completed or SessionStatus.Rescheduled)
            //SessionStatus = mats.Sessions.Select(s => s.Status)
        };
    }

    public static AvailableMentorForBookingResponse CreateAvailableMentorForBookingResponse(
        User user, Schedules earliestWorkingSchedule)
    {
        return new AvailableMentorForBookingResponse
        {
            MentorId = user.Id,
            MentorName = user.FullName,
            MentorExpertise = user.UserExpertises.Select(ue => ue.Expertise!.Name).ToList(),
            MentorAvatarUrl = user.ProfilePhotoUrl,
            WorkingStartTime = earliestWorkingSchedule.StartHour,
            WorkingEndTime = earliestWorkingSchedule.EndHour
        };
    }

    public static SessionSlotStatusResponse ToSessionSlotStatusResponse(this Sessions booking)
    {
        var mats = booking.TimeSlot;
        return new SessionSlotStatusResponse
        {
            SessionId = booking.Id,
            SlotId = booking.Id,
            MentorId = mats.Schedules.MentorId,
            Day = mats.Date,
            StartTime = mats.StartTime,
            EndTime = mats.EndTime,
            BookingStatus = booking.Status
        };
    }

    public static GetAllRequestByLearnerResponse ToGetAllRequestLearnerResponse(this Sessions booking)
    {
        var mats = booking.TimeSlot;
        return new GetAllRequestByLearnerResponse
        {
            SessionId = booking.Id,
            SlotId = booking.Id,
            MentorName = booking.TimeSlot.Schedules.Mentor.FullName,
            Expirtise = booking.TimeSlot.Schedules.Mentor.UserExpertises.Select(ue => ue.Expertise!.Name).ToList(),
            MentorAvatarUrl = booking.TimeSlot.Schedules.Mentor.ProfilePhotoUrl,
            SessionType = booking.Type,
            Day = mats.Date,
            StartTime = mats.StartTime,
            EndTime = mats.EndTime,
            BookingStatus = booking.Status
        };
    }
}