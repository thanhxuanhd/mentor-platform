
using System.Text.Json.Serialization;

namespace Domain.Enums;

[JsonConverter(typeof(JsonStringEnumConverter<SessionStatus>))]
public enum SessionStatus
{
    Pending = 0,
    Confirmed = 1,
    Completed = 2,
    Canceled = 3
}
