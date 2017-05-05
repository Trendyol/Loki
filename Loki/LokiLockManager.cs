namespace Loki
{
    internal class LokiLockManager
    {
        private readonly LokiLockHandler _lokiLockHandler = LokiConfiguration.LokiLockHandler;
        private readonly object _lock = new object();

        public bool Lock(int expiryFromSeconds, string uniqueKey = null)
        {
            lock (_lock)
            {
                string serviceKey = LokiConfiguration.ServiceKey;

                if (uniqueKey != null)
                {
                    serviceKey += uniqueKey;
                }

                return _lokiLockHandler.Lock(serviceKey, expiryFromSeconds);
            }
        }

        public void Release(string uniqueKey = null)
        {
            lock (_lock)
            {
                string serviceKey = LokiConfiguration.ServiceKey;

                if (uniqueKey != null)
                {
                    serviceKey += uniqueKey;
                }

                _lokiLockHandler.Release(serviceKey);
            }
        }
    }
}