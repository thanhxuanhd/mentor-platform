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

        public static string BodyUpdatedNotificationApplication(string adminName, Guid mentorId)
        {
            return $@"
            Hello {adminName},
            {Environment.NewLine}
            Mentor with ID {mentorId} has updated their application.
            {Environment.NewLine}
            Thank you,
            Support Team";
        }
    }
}
