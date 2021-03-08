using System.Collections.Generic;
using System.Linq;

namespace Exception.Core
{
    public class ResultBuilder<TResult> where TResult : ResultBase
    {
        protected readonly TResult Result;

        internal ResultBuilder(TResult result)
        {
            Result = result;
        }

        public ResultBuilder<TResult> WithInfo(string translatedMessage, object tokens = null)
        {
            Result.Messages = Result.Messages.Concat(new[] { Message.Info(translatedMessage, tokens) }).ToArray();
            return this;
        }

        public ResultBuilder<TResult> WithWarning(string translatedMessage, object tokens = null)
        {
            Result.Messages = Result.Messages.Concat(new[] { Message.Warning(translatedMessage, tokens) }).ToArray();
            return this;
        }

        public ResultBuilder<TResult> WithValidationError(string translatedMessage, object tokens = null)
        {
            Result.Messages = Result.Messages.Concat(new[] { Message.ValidationError(translatedMessage, tokens) }).ToArray();
            return this;
        }

        public ResultBuilder<TResult> WithMessages(params Message[] messages)
        {
            Result.Messages = Result.Messages.Concat(messages).ToArray();
            return this;
        }

        public ResultBuilder<TResult> WithMessages(IEnumerable<Message> messages)
        {
            return WithMessages(messages.ToArray());
        }

        public static implicit operator TResult(ResultBuilder<TResult> builder)
        {
            return builder.Result;
        }
    }
}