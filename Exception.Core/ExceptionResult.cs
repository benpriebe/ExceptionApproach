using System.Text.Json.Serialization;

namespace Exception.Core
{
    public class ExceptionResult
    {
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string Message { get; set; }
        
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public Message[] Extras { get; set; }
    }
}