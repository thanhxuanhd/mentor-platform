using System.Text.Json.Serialization;

namespace Domain.Enums;

[JsonConverter(typeof(JsonStringEnumConverter<CourseState>))]
public enum CourseState
{
    Draft,
    Published,
    Archived
}