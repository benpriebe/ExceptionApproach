using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace Exception.Core
{
    #region ResultBase 
    
    public abstract class ResultBase
    {
        protected ResultBase()
        {
            HasContent = false;
            Messages = null;
        }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public IEnumerable<Message> Messages { get; internal set; }
        
        [JsonIgnore]
        public bool HasContent { get; internal set; }
    }
    
    #endregion ResultBase 

    #region Result

    public class Result : ResultBase
    {
        
        public Result WithInfo(string translatedMessage, object tokens = null)
        {
            Messages ??= Array.Empty<Message>();
            Messages = Messages.Concat(new[] { Message.Info(translatedMessage, tokens) }).ToArray();
            return this;
        }

        public Result WithWarning(string translatedMessage, object tokens = null)
        {
            Messages ??= Array.Empty<Message>();
            Messages = Messages.Concat(new[] { Message.Warning(translatedMessage, tokens) }).ToArray();
            return this;
        }

        public Result WithValidationError(string translatedMessage, object tokens = null)
        {
            Messages ??= Array.Empty<Message>();
            Messages = Messages.Concat(new[] { Message.ValidationError(translatedMessage, tokens) }).ToArray();
            return this;
        }

        public Result WithMessages(params Message[] messages)
        {
            Messages ??= Array.Empty<Message>();
            Messages = Messages.Concat(messages).ToArray();
            return this;
        }

        public Result WithMessages(IEnumerable<Message> messages)
        {
            return WithMessages(messages.ToArray());
        }

    }

    #endregion Result

    #region Result<TEntity>

    public class Result<TEntity> : ResultBase
    {
        private readonly TEntity value;
        
        public Result(TEntity value)
        {
            HasContent = true;
            Value = value;
        }
        
        public Result<TEntity> WithInfo(string translatedMessage, object tokens = null)
        {
            Messages ??= Array.Empty<Message>();
            Messages = Messages.Concat(new[] { Message.Info(translatedMessage, tokens) }).ToArray();
            return this;
        }

        public Result<TEntity> WithWarning(string translatedMessage, object tokens = null)
        {
            Messages ??= Array.Empty<Message>();
            Messages = Messages.Concat(new[] { Message.Warning(translatedMessage, tokens) }).ToArray();
            return this;
        }

        public Result<TEntity> WithValidationError(string translatedMessage, object tokens = null)
        {
            Messages ??= Array.Empty<Message>();
            Messages = Messages.Concat(new[] { Message.ValidationError(translatedMessage, tokens) }).ToArray();
            return this;
        }

        public Result<TEntity> WithMessages(params Message[] messages)
        {
            Messages ??= Array.Empty<Message>();
            Messages = Messages.Concat(messages).ToArray();
            return this;
        }

        public Result<TEntity> WithMessages(IEnumerable<Message> messages)
        {
            return WithMessages(messages.ToArray());
        }

        [JsonPropertyName("data")]
        public TEntity Value
        {
            get => value;
            init
            {
                if (!HasContent && !Equals(value, default(TEntity)))
                {
                    throw new ArgumentException("You cannot set a value on the result object when HasContent is false.");
                }

                this.value = value;
            }
        }
    }

    #endregion
}