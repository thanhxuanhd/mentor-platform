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

        public static string BodyUpdatedNotificationApplication(string adminName, string mentorName)
        {
            return $@"
            Hello {adminName},
            <br>
            Mentor {mentorName} has updated their application.
            <br><br>
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

        public const string SUBJECT_SESSION_ACCEPTED = "Booked Session Successfully";

        public static string BodySessionAcceptedEmail(Guid requestId) => $@"
<!DOCTYPE html>
<html lang=""en"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>Booked Session Successfully</title>
    <style>
        body, html {{
            margin: 0;
            padding: 0;
            font-family: Arial, sans-serif;
            background-color: #f4f4f4;
        }}
        .container {{
            max-width: 600px;
            margin: 30px auto;
            background-color: #ffffff;
            border-radius: 10px;
            box-shadow: 0 2px 8px rgba(0,0,0,0.1);
            padding: 30px;
        }}
        h1 {{
            color: #2d7a2d;
            text-align: center;
            font-size: 24px;
        }}
        p {{
            color: #333;
            margin: 15px 0;
            line-height: 1.6;
        }}
        .footer {{
            margin-top: 30px;
            text-align: center;
            font-size: 12px;
            color: #999;
        }}
    </style>
</head>
<body>
    <div class=""container"">
        <h1>📌 Booked Session Successfully</h1>
        <p><strong>Sender:</strong> MentorConnect Project</p>
        <p>Your booked sessions has been approved by the mentor.</p>
        <p>Please check in the system for details. Your session Request ID is: <strong>{requestId}</strong></p>
        <p>We’re excited to support your learning journey!</p>
        <div class=""footer"">
            <p>This is an automated message. Please do not reply.</p>
        </div>
    </div>
</body>
</html>";


        public const string SUBJECT_SESSION_CANCELLED = "Booked Session Failed";

        public static string BodySessionCancelledEmail(Guid requestId) => $@"
<!DOCTYPE html>
<html lang=""en"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>Booked Session Failed</title>
    <style>
        body, html {{
            margin: 0;
            padding: 0;
            font-family: Arial, sans-serif;
            background-color: #f4f4f4;
        }}
        .container {{
            max-width: 600px;
            margin: 30px auto;
            background-color: #ffffff;
            border-radius: 10px;
            box-shadow: 0 2px 8px rgba(0,0,0,0.1);
            padding: 30px;
        }}
        h1 {{
            color: #cc0000;
            text-align: center;
            font-size: 24px;
        }}
        p {{
            color: #333;
            margin: 15px 0;
            line-height: 1.6;
        }}
        .footer {{
            margin-top: 30px;
            text-align: center;
            font-size: 12px;
            color: #999;
        }}
    </style>
</head>
<body>
    <div class=""container"">
        <h1>⚠️ Booked Session Failed</h1>
        <p><strong>Sender:</strong> MentorConnect Project</p>
        <p>Your booked sessions has been rejected by the mentor.</p>
        <p>Please check in the system for more information. Your session Request ID is: <strong>{requestId}</strong></p>
        <p>Feel free to book a new session or contact support for assistance.</p>
        <div class=""footer"">
            <p>This is an automated message. Please do not reply.</p>
        </div>
    </div>
</body>
</html>";


        public const string SUBJECT_SESSION_RESCHEDULED = "Your Mentoring Session Has Been Rescheduled";

        public static string BodySessionRescheduledEmail(Guid id, DateOnly date, TimeOnly startTime, TimeOnly endTime, string? reason) => $@"
<!DOCTYPE html>
<html lang=""en"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>Mentoring Session Rescheduled</title>
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
            color: #3366cc;
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
        <h1>Session Rescheduled</h1>
        <p>Hello,</p>
        <p>Your mentoring session (Request ID: <strong>{id}</strong>) has been successfully rescheduled.</p>
        <p><strong>New Schedule:</strong> {date:MMM dd, yyyy}, from {startTime} to {endTime}</p>
        {(string.IsNullOrWhiteSpace(reason) ? "" : $"<p><strong>Reason:</strong> {reason}</p>")}
        <p>If you have any questions or concerns, feel free to contact our support team.</p>
        <p>Thank you,</p>
        <p>The Mentoring Team</p>
        <div class=""footer"">
            <p>This is an automated message. Please do not reply.</p>
        </div>
    </div>
</body>
</html>";


    }
}