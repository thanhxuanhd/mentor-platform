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

        public const string SUBJECT_REQUEST_APPLICATION_INFO = "Request Application Information";
        public static string BodyRequestApplicationInfoEmail(string mentorName)
        {
            return $@"
            Dear {mentorName},
            {Environment.NewLine}
            We have requested additional information regarding your mentor application. Please check your profile for details.
            {Environment.NewLine}
            Best regards,
            Mentor Platform Team";
        }
    }
}
