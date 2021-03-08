namespace Exception.Core.Exceptions
{
    public class ValidationClientException : ClientException
    {
        public ValidationClientException(Validator validator) : base("TODO: Resx this message")
        {
            Messages = validator.Errors;
        }
    }
}