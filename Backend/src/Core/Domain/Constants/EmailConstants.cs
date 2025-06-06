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
    }
}
