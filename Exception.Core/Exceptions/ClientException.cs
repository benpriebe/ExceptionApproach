using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Exception.Core.Exceptions
{
    public class ClientException : System.Exception
    {
        public ClientException(params Message[] messages)
        {
            Messages = messages;
        }

        public ClientException(IEnumerable<Message> messages)
        {
            Messages = messages;
        }
        
        [JsonInclude]
        public IEnumerable<Message> Messages { get; internal set; }
    }
}