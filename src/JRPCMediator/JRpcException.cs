namespace JRpcMediator
{
    public class JRpcException : Exception
    {
        public JRpcError RpcError { get; init; }

        public JRpcException(JRpcError error) : base(error.Message)
        {
            RpcError = error;
        }
    }
}
