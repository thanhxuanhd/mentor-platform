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

        public const string SUBJECT_MENTOR_APPLICATION_DECISION = "Mentor Application Decision";
        public static string BodyMentorApplicationDecisionEmail(string mentorName, string status, string? note)
        {
            if (string.IsNullOrEmpty(note))
            {
                note = "No additional notes provided.";
            }
            
            return $@"
            Dear {mentorName},
            {Environment.NewLine}
            Your mentor application has been {status.ToLower()}.
            {Environment.NewLine}
            Note: {note}
            {Environment.NewLine}
            Thank you for your interest in becoming a mentor.
            {Environment.NewLine}
            Best regards,
            Mentor Platform Team";
        }
    }
}
