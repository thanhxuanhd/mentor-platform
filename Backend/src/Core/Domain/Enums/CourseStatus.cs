using System.Text.Json.Serialization;

namespace Domain.Enums;

[JsonConverter(typeof(JsonStringEnumConverter<CourseStatus>))]
public enum CourseStatus
{
    Draft,
    Published,
    Archived
}