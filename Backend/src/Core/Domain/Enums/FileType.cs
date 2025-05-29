using System.Text.Json.Serialization;

namespace Domain.Enums;

[JsonConverter(typeof(JsonStringEnumConverter<FileType>))]
public enum FileType
{
    Pdf = 0,
    Video = 1,
    Audio = 3,
    Image = 4,
    ExternalWebAddress = 5
}