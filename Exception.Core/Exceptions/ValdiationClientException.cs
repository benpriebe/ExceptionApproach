namespace Exception.Core.Exceptions
{
    public class ValidationClientException : ClientException
    {
        public ValidationClientException(Validator validator) : base(validator.Errors)
        {
        }
    }
}