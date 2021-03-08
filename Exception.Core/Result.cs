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
            Messages = Array.Empty<Message>();
        }

        public ResultBase(params Message[] messages)
        {
            Messages = messages;
        }

        public ResultBase(IEnumerable<Message> messages)
        {
            Messages = messages;
        }
        
       
        public IEnumerable<Message> Messages { get; internal set; }
        
        [JsonIgnore]
        public bool HasContent { get; internal set; }
    }
    
    #endregion ResultBase 

    #region Result

    public class Result : ResultBase
    {
        
        /// <summary>
        /// Creates a success <see cref="Result"/> with no content.
        /// </summary>
        public static ResultBuilder<Result> New()
        {
            var result = new Result {  HasContent = false };
            return new ResultBuilder<Result>(result);
        }
    }

    #endregion Result

    #region Result<TEntity>

    public class Result<TEntity> : ResultBase
    {
        private readonly TEntity value;

        /// <summary>
        /// Creates a success <see cref="Result"/> with no content.
        /// </summary>
        public static ResultBuilder<Result<TEntity>> New(TEntity value)
        {
            var result = new Result<TEntity>(value);
            return new ResultBuilder<Result<TEntity>>(result);
        }
        
        public Result(TEntity value)
        {
            HasContent = true;
            Value = value;
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

    public enum ErrorMode
    {
        FirstError,
        AllErrors
    }

}