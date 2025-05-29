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

        public static string BodyUpdatedNotificationApplication(string adminName, Guid mentorId)
        {
            return $@"
            Hello {adminName},
            <br>
            Mentor with ID {mentorId} has updated their application.
            <br><br>
            Thank you,
            Support Team";
        }
    }
}
