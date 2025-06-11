using System.Text.Json.Serialization;

namespace Domain.Enums;
[JsonConverter(typeof(JsonStringEnumConverter<SessionStatus>))]
public enum SessionStatus
{
    Pending = 0,
    Approved = 1,
    Completed = 2,
    Cancelled = 3,
    Rescheduled = 4,
}
