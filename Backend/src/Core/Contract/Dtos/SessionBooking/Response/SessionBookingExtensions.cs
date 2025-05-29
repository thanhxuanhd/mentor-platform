using Domain.Entities;

namespace Contract.Dtos.SessionBooking.Response;

public static class SessionBookingExtensions
{
    public static AvailableMentorForBookingResponse ToAvailableMentorForBookingResponse(
        this MentorAvailableTimeSlot mats)
    {
        return new AvailableMentorForBookingResponse
        {
            MentorId = mats.MentorId,
            MentorName = mats.Mentor.FullName,
            MentorExpertise = mats.Mentor.UserExpertises.Select(ue => ue.Expertise!.Name).ToList(),
            MentorAvatarUrl = mats.Mentor.ProfilePhotoUrl,
            WorkingEndTime = mats.Schedule.StartTime,
            WorkingStartTime = mats.Schedule.EndTime
        };
    }

    public static SessionSlotStatusResponse ToSessionSlotStatusResponse(this Booking booking)
    {
        var mats = booking.TimeSlot;
        return new SessionSlotStatusResponse
        {
            SlotId = booking.Id,
            MentorId = mats.MentorId,
            BookingStatus = mats.Status,
            StartTime = mats.StartTime,
            EndTime = mats.EndTime
        };
    }
}