using System.Text.Json.Serialization;

namespace Domain.Enums;

[JsonConverter(typeof(JsonStringEnumConverter<FileType>))]
public enum FileType
{
    Pdf = 0,
    Video = 1,
    ExternalWebAddress = 2
}