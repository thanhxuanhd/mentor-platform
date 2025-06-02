namespace Domain.Constants
{
    public static class EmailConstants
    {
        public const string SUBJECT_RESET_PASSWORD = "Reset Your Password";

        public static string BodyResetPasswordEmail(string email, string newPassword)
        {
            return $@"
            Hi {email},
            {Environment.NewLine}
            Your password has been reset successfully.
            Your new password is:
            {Environment.NewLine}
            {newPassword}
            {Environment.NewLine}
            Please login and change it as soon as possible for security reasons.
            {Environment.NewLine}
            Thank you,
            Support Team";
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