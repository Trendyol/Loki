namespace Loki
{
    internal class LokiLockManager
    {
        private readonly LokiLockHandler _lokiLockHandler = LokiConfiguration.LokiLockHandler;
        private readonly object _lock = new object();

        public bool Lock(int expiryFromSeconds)
        {
            lock (_lock)
            {
                return _lokiLockHandler.Lock(LokiConfiguration.ServiceKey, expiryFromSeconds);
            }
        }

        public void Release()
        {
            lock (_lock)
            {
                _lokiLockHandler.Release(LokiConfiguration.ServiceKey);
            }
        }
    }
}