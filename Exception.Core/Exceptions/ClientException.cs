using System;
using System.Collections.Generic;
using System.Linq;

namespace Exception.Core.Exceptions
{
    public class ClientException : System.Exception
    {
        public ClientException(string translatedMessage, object tokens = null)
            : base(Core.Message.Error(translatedMessage, tokens).Content)
        {
        }

        public ClientException(params Message[] messages)
        {
            Messages = messages;
        }

        public ClientException(IEnumerable<Message> messages)
        {
            Messages = messages;
        }
        
        public IEnumerable<Message> Messages { get; internal set; }
        
        public ClientException WithInfo(string translatedMessage, object tokens = null)
        {
            Messages ??= Array.Empty<Message>();
            Messages = Messages.Concat(new[] { Core.Message.Info(translatedMessage, tokens) }).ToArray();
            return this;
        }

        public ClientException WithWarning(string translatedMessage, object tokens = null)
        {
            Messages ??= Array.Empty<Message>();
            Messages = Messages.Concat(new[] { Core.Message.Warning(translatedMessage, tokens) }).ToArray();
            return this;
        }

        public ClientException WithValidationError(string translatedMessage, object tokens = null)
        {
            Messages ??= Array.Empty<Message>();
            Messages = Messages.Concat(new[] { Core.Message.ValidationError(translatedMessage, tokens) }).ToArray();
            return this;
        }
        
        public ClientException WithError(string translatedMessage, object tokens = null)
        {
            Messages ??= Array.Empty<Message>();
            Messages = Messages.Concat(new[] { Core.Message.ValidationError(translatedMessage, tokens) }).ToArray();
            return this;
        }
        
        public ClientException WithMessages(params Message[] messages)
        {
            Messages ??= Array.Empty<Message>();
            Messages = Messages.Concat(messages).ToArray();
            return this;
        }

        public ClientException WithMessages(IEnumerable<Message> messages)
        {
            return WithMessages(messages.ToArray());
        }
    }
}