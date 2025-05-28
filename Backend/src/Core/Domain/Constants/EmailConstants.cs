namespace Domain.Constants
{
    public static class EmailConstants
    {
        public const string SUBJECT_RESET_PASSWORD = "Reset Your Password";

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
    }
}
