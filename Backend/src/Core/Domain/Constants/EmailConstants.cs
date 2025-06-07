namespace Domain.Constants
{
    public static class EmailConstants
    {
        public const string SUBJECT_RESET_PASSWORD = "Reset Your Password";

        public const string SUBJECT_UPDATE_APPLICATION = "Mentor has updated application";
        public static string BodyResetPasswordEmail(string email, string newPassword)
        {
            return $@"
            Hi {email},
            <br>
            Your password has been reset successfully.
            Your new password is:
            <br>
            {newPassword}
            <br>
            Please login and change it as soon as possible for security reasons.
            <br>
            Thank you,
            Support Team";
        }

        public const string SUBJECT_REQUEST_APPLICATION_INFO = "Request Application Information";
        public static string BodyRequestApplicationInfoEmail(string mentorName)
        {
            return $@"
            Dear {mentorName},<br><br>
            Thank you for your interest in becoming a mentor. We have received your application and are currently reviewing it.
            We have requested additional information regarding your mentor application. Please check your profile for details.<br><br>
            Best regards,<br>
            Mentor Platform Team";
        }

        public const string SUBJECT_MENTOR_APPLICATION_DECISION = "Mentor Application Decision";
        public static string BodyMentorApplicationDecisionEmail(string mentorName, string status, string? note)
        {
            if (string.IsNullOrEmpty(note))
            {
                note = "No additional notes provided.";
            }

            return $@"
            Dear {mentorName},<br><br>

            Your mentor application has been {status.ToLower()}.<br><br>

            Note: {note}<br>
            Thank you for your interest in becoming a mentor.<br><br>
            
            Best regards,<br>
            Mentor Platform Team";
        }

        public static string BodyUpdatedNotificationApplication(string adminName, string mentorName)
        {
            return $@"
            Hello {adminName},
            <br>
            Mentor {mentorName} has updated their application.
            <br><br>
            Thank you,
            Support Team";
        }

        public const string SUBJECT_MENTOR_UPDATED_SCHEDULE = "Mentor Schedule Updated";

        public static string BodyMentorUpdatedScheduleEmail(string learnerName, string mentorName)
        {
            return $@"
            Hi {learnerName}, <br><br>

            Mentor {mentorName} has updated their schedule. Please check the updated availability on the platform.<br><br>

            All your pending sessions with this mentor have been cancelled. You can reschedule them at your convenience.<br><br>

            Thank you,<br>
            MentorPlatform Team";
        }

        public static string BodyMeetingBookingCancelledEmail()
        {
            return string.Empty;
        }

        public const string SUBJECT_MEETING_BOOKING_CONFIRMATION = "Your Meeting Session is Confirmed";

        public static string BodyMeetingBookingConfirmationEmail(
            string recipientName,
            DateTime meetingDateTime,
            string mentorName)
        {
            var meetingYearMonthDay = meetingDateTime.ToString("MM-dd-yyyy HH:mm");

            return $"""
                    Hi {recipientName},
                    </br>
                    This email confirms your booking for the following meeting session:
                    </br>
                    When: {meetingYearMonthDay}
                    Organized by: {mentorName}
                    </br>
                    We look forward to your participation.
                    </br>
                    If you need to make any changes or have questions, please reply to this email or contact the organizer.
                    </br>
                    Thank you,
                    Support Team
                    """;
        }
    }
}