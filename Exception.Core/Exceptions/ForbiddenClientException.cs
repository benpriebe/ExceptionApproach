namespace Exception.Core.Exceptions
{
    public class ForbiddenClientException : ClientException
    {
        public ForbiddenClientException() : base(Core.Message.Forbidden())
        {
        }
    }
}