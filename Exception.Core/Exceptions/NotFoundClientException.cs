namespace Exception.Core.Exceptions
{
    public class NotFoundClientException<TEntity> : ClientException
    {
        public NotFoundClientException(object identity) : base(Core.Message.NotFound<TEntity>(identity).Content)
        {
        }
    }
}