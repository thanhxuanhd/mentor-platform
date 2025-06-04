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
    }
}
