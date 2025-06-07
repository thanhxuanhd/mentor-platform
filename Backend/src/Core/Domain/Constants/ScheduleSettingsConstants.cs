namespace Domain.Constants;

public static class ScheduleSettingsConstants
{
    public const int MinSessionDuration = 30;
    public const int MaxSessionDuration = 90;
    public const int MinBufferTime = 0;
    public const int MaxBufferTime = 60;
    public static readonly TimeOnly DefaultStartTime = new(9, 0); 
    public static readonly TimeOnly DefaultEndTime = new(17, 0); 
    public const int DefaultSessionDuration = 60; 
    public const int DefaultBufferTime = 15;

}
