
<<<<<<< HEAD
using System.Text.Json.Serialization;

namespace Domain.Enums;

[JsonConverter(typeof(JsonStringEnumConverter<SessionStatus>))]
=======
namespace Domain.Enums;

>>>>>>> origin
public enum SessionStatus
{
    Pending = 0,
    Approved = 1,
    Completed = 2,
    Canceled = 3,
    Rescheduled = 4,
}
