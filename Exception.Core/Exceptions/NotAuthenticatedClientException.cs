namespace Exception.Core.Exceptions
{
    public class NotAuthenticatedClientException : ClientException
    {
        public NotAuthenticatedClientException() : base(Core.Message.Unauthorized().Content)
        {
        }
    }
}