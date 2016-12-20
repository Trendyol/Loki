namespace Loki
{
    public abstract class LokiLockHandler
    {
        protected LokiLockHandler SecondaryLockHandler;

        internal void SetHandler(LokiLockHandler lockHandler)
        {
            SecondaryLockHandler = lockHandler;
        }

        public abstract bool Lock(string tenantType, int expiryFromSeconds);
        public abstract void Release(string tenantType);
    }
}