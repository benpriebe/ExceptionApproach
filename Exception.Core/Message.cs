using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Threading;

namespace Exception.Core
{
    public class Message
    {
        internal Message() {}
        
        public Message(
            MessageType type, 
            string translatedMessage, 
            object tokens = null) : this(type, translatedMessage, Thread.CurrentThread.CurrentUICulture.Name, tokens)
        {
        }

        public Message(
            MessageType type,
            string translatedMessage,
            string locale,
            object tokens = null)
        {
            using (EphemeralUiCulture.ChangeLocale(locale))
            {
                Type = type;
                Content = Format(LowerCamelCaseTokens(translatedMessage), tokens);
            }
        }

        /// <summary>
        ///     The type of message.
        /// </summary>
        public MessageType? Type { get; internal set; }

        /// <summary>
        ///     The translated tokenized template.
        /// </summary>
        /// <example>
        ///     "The first name - Ben - must contain - 8 - characters or more."
        /// </example>
        public string Content { get; internal set; }

        /// <summary>
        ///     Converts a named token template string into an indexed based token string that can be evaluated at runtime.
        /// </summary>
        /// <param name="template">the named token string</param>
        /// <param name="tokens">the token object</param>
        /// <returns>the interpolated string</returns>
        internal static string Format(string template, object tokens)
        {
            if (tokens == null) return template;
            var propertyInfos = tokens.GetType().GetProperties();

            var tokenValues = new List<object>();
            var message = propertyInfos.Aggregate(template, (memo, propertyInfo) =>
            {
                var tokenKey = propertyInfo.Name;
                var tokenValue = propertyInfo.GetValue(tokens);
                tokenValues.Add(tokenValue);

                var pattern = $@"{{(?<Token>\s*?{tokenKey}\s*?)(?<Format>:[^}}]+)?}}";
                var replacement = Regex.Replace(memo, pattern, match =>
                {
                    var format = match.Groups["Format"].Value;
                    return $"{{{tokenValues.Count - 1}{format}}}";
                }, RegexOptions.IgnoreCase | RegexOptions.Multiline);

                return replacement;
            });

            // ensure that all named tokens in the template have a token value provided.
            var untokenizedTokens = Regex.Matches(message, "(?<!{){[A-Z][^{}]+?}(?!})", RegexOptions.IgnoreCase).ToArray();
            if (untokenizedTokens.Any())
            {
                var missingTokens = string.Join(", ", untokenizedTokens.Select(match => match.Value));
                var error =
                    $"The message template references tokens that have not been supplied.{Environment.NewLine}template: {template}{Environment.NewLine}missing token values: {missingTokens}";

                // todo: get the environment from somewhere.
                // var environment = "Production";
                //
                // if (!string.Equals(environment, "Production", StringComparison.OrdinalIgnoreCase))
                //     throw new ArgumentOutOfRangeException(error);

                return template;
            }

            message = string.Format(message, tokenValues.ToArray());
            return message;
        }

        private static string LowerCamelCaseTokens(string template)
        {
            return Regex.Replace(template, "(?<!{){(?<Token>[^{}]+?)}(?!})", match =>
            {
                var token = match.Groups["Token"].Value;
                return $"{{{char.ToLowerInvariant(token[0])}{token.Substring(1)}}}";
            });
        }


        #region Creation Methods

        public static Message ValidationError(string translatedMessage, object tokens = null)
        {
            return new(MessageType.ValidationError, translatedMessage, tokens);
        }

        public static Message Error(string translatedMessage, object tokens = null)
        {
            return new(MessageType.Error, translatedMessage, tokens);
        }

        public static Message Warning(string translatedMessage, object tokens = null)
        {
            return new(MessageType.Warning, translatedMessage, tokens);
        }

        public static Message Info(string translatedMessage, object tokens = null)
        {
            return new(MessageType.Information, translatedMessage, tokens);
        }

        public static Message Unauthorized(string translatedMessage, object tokens = null)
        {
            return new(MessageType.Unauthorized, translatedMessage, tokens);
        }

        public static Message Forbidden(string translatedMessage, object tokens = null)
        {
            return new(MessageType.Forbidden, translatedMessage, tokens);
        }

        public static Message Unauthorized()
        {
            return new(
                MessageType.Unauthorized,
                i18n.Message.Unauthorized
            );
        }

        public static Message Forbidden()
        {
            return new(
                MessageType.Forbidden,
                i18n.Message.Forbidden
            );
        }

        public static Message NotFound<TEntity>(object identity)
        {
            var sourceType = typeof(TEntity);
            if (sourceType.IsGenericType)
                sourceType = sourceType.GenericTypeArguments[0];
            return new Message(
                MessageType.NotFound,
                i18n.Message.NotFound,
                new
                {
                    type = sourceType.Name,
                    id = Convert.ToString(identity)
                });
        }

        #endregion
    }
}