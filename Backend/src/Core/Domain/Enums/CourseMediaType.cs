using System.Text.Json.Serialization;

namespace Domain.Enums;

[JsonConverter(typeof(JsonStringEnumConverter<CourseMediaType>))]
public enum CourseMediaType
{
    Pdf = 0,
    Video = 1,
    ExternalWebAddress = 2
}