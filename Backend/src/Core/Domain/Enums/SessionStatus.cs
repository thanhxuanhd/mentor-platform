
using System.Text.Json.Serialization;

namespace Domain.Enums;

[JsonConverter(typeof(JsonStringEnumConverter<SessionStatus>))]
public enum SessionStatus
{
    Available = 0,
    Processing = 1,
    Confirmed = 2,
    Expired = 3
}
