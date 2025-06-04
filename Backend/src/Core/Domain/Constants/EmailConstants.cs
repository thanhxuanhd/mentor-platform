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

        public const string SUBJECT_SESSION_ACCEPTED = "Your Mentoring Session Has Been Accepted";

        public static string BodySessionAcceptedEmail(Guid requestId) => $@"
<!DOCTYPE html>
<html lang=""en"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>Mentoring Session Accepted</title>
    <style>
        body, html {{
            margin: 0;
            padding: 0;
            font-family: Arial, sans-serif;
            line-height: 1.6;
        }}
        .container {{
            max-width: 600px;
            margin: 20px auto;
            padding: 20px;
            border: 1px solid #ccc;
            border-radius: 10px;
            background-color: #f9f9f9;
        }}
        h1 {{
            font-size: 24px;
            text-align: center;
            color: #333;
        }}
        p {{
            margin-bottom: 20px;
            color: #555;
        }}
        .footer {{
            margin-top: 20px;
            text-align: center;
            color: #999;
        }}
    </style>
</head>
<body>
    <div class=""container"">
        <h1>Session Confirmed</h1>
        <p>Hello,</p>
        <p>Your mentoring session (Request ID: <strong>{requestId}</strong>) has been successfully accepted.</p>
        <p>We look forward to seeing you at the scheduled time. If you have any questions or concerns, feel free to reach out to our support team.</p>
        <p>Thank you,</p>
        <p>The Mentoring Team</p>
        <div class=""footer"">
            <p>This is an automated message. Please do not reply.</p>
        </div>
    </div>
</body>
</html>";

        public const string SUBJECT_SESSION_CANCELLED = "Your Mentoring Session Has Been Cancelled";

        public static string BodySessionCancelledEmail(Guid requestId) => $@"
<!DOCTYPE html>
<html lang=""en"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>Mentoring Session Accepted</title>
    <style>
        body, html {{
            margin: 0;
            padding: 0;
            font-family: Arial, sans-serif;
            line-height: 1.6;
        }}
        .container {{
            max-width: 600px;
            margin: 20px auto;
            padding: 20px;
            border: 1px solid #ccc;
            border-radius: 10px;
            background-color: #f9f9f9;
        }}
        h1 {{
            font-size: 24px;
            text-align: center;
            color: #333;
        }}
        p {{
            margin-bottom: 20px;
            color: #555;
        }}
        .footer {{
            margin-top: 20px;
            text-align: center;
            color: #999;
        }}
    </style>
</head>
<body>
    <div class=""container"">
        <h1>Session Confirmed</h1>
        <p>Hello,</p>
        <p>Your mentoring session (Request ID: <strong>{requestId}</strong>) has been successfully cancelled.</p>
        <p>We look forward to seeing you at the scheduled time. If you have any questions or concerns, feel free to reach out to our support team.</p>
        <p>Thank you,</p>
        <p>The Mentoring Team</p>
        <div class=""footer"">
            <p>This is an automated message. Please do not reply.</p>
        </div>
    </div>
</body>
</html>";

        public const string SUBJECT_SESSION_RESCHEDULED = "Your session has been rescheduled";
        public static string BodySessionRescheduledEmail(Guid id, DateOnly date, TimeOnly startTime, TimeOnly endTime, string? reason)
        {
            return $"Your session (ID: {id}) has been rescheduled to {date:MMM dd, yyyy} from {startTime} to {endTime}. Reason: {reason}";
        }

    }
}