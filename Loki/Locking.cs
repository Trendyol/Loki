using System;

namespace Loki
{
    public class Locking
    {
        private static readonly Lazy<Locking> _Instance = new Lazy<Locking>(() => new Locking());
        private static readonly LokiLockManager LokiLockManager = new LokiLockManager();

        public static Locking Instance => _Instance.Value;

        private Locking()
        {
        }

        public void ExecuteWithinLock(Action action, int expiryFromSeconds)
        {
            bool isLocked = LokiLockManager.Lock(expiryFromSeconds);

            if (isLocked)
            {
                action.Invoke();
                LokiLockManager.Release();
            }
        }
    }
}