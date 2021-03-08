using System.Text.Json.Serialization;
using Exception.Core.Json;

namespace Exception.Core
{
    [JsonConverter(typeof(KebabCaseEnumConverter))]
    public enum MessageType 
    {
        Information,
        Warning,
        Error,
        ValidationError, 
        Unauthorized, // user is not authenticated
        Forbidden, // user is authenticated but access is forbidden
        NotFound
    }
}