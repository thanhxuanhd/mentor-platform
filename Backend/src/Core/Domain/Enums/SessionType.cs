using System.Text.Json.Serialization;

namespace Domain.Enums;

[JsonConverter(typeof(JsonStringEnumConverter<SessionType>))]
public enum SessionType
{
    Virtual,
    OneOnOne,
    Onsite
}