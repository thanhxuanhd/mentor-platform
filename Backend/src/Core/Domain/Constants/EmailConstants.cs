namespace Domain.Constants
{
    public static class EmailConstants
    {
        public const string SUBJECT_RESET_PASSWORD = "Reset Your Password";

        public static string BodyResetPasswordEmail(string email, string newPassword)
        {
            return $@"
            Hi {email},

            Your password has been reset successfully.
            Your new password is: {newPassword}

            Please login and change it as soon as possible for security reasons.

            Thank you,
            Support Team";
        }
    }
}
