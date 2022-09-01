namespace JRpcMediator.Client
{
    class IdUtil
    {
        private static int _current = 1;
        public static int NextId()
        {
            return _current++;
        }
    }
}