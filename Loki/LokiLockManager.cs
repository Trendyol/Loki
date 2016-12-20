namespace Loki
{
    internal class LokiLockManager
    {
        private readonly LokiLockHandler _lokiLockHandler = LokiConfiguration.LokiLockHandler;

        public bool Lock(int expiryFromSeconds)
        {
            return _lokiLockHandler.Lock(LokiConfiguration.TenantType, expiryFromSeconds);
        }

        public void Release()
        {
            _lokiLockHandler.Release(LokiConfiguration.TenantType);
        }
    }
}