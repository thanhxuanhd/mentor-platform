using System.Text.Json.Serialization;

namespace Domain.Enums;

[JsonConverter(typeof(JsonStringEnumConverter<CourseDifficulty>))]
public enum CourseDifficulty
{
    Beginner,
    Intermediate,
    Advanced
}