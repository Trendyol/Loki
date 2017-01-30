using System;
using System.Net;
using System.Threading.Tasks;

namespace Loki.Redis
{
    public class RedisLokiLockHandler : LokiLockHandler
    {
        private static IRedisStore _redisStore;

        public RedisLokiLockHandler(EndPoint[] redisEndPoints)
        {
            _redisStore = RedisStore.Instance.Initialize(redisEndPoints);
        }

        public override bool Lock(string tenantType, int expiryFromSeconds)
        {
            bool isLocked = false;

            try
            {
                string isAnyLocked = _redisStore.Get(tenantType);

                if (String.IsNullOrEmpty(isAnyLocked))
                {
                    isLocked = _redisStore.Set(tenantType, "locked", expiryFromSeconds);

                    // When occurs any network problem within Redis, it could be provided consistent locking with secondary handler.
                    if (SecondaryLockHandler != null)
                    {
                        Task.Factory.StartNew(() => SecondaryLockHandler.Lock(tenantType, expiryFromSeconds));
                    }
                }
            }
            catch (Exception ex)
            {
                if (SecondaryLockHandler != null)
                {
                    isLocked = SecondaryLockHandler.Lock(tenantType, expiryFromSeconds);
                }
            }

            return isLocked;
        }

        public override void Release(string tenantType)
        {
            try
            {
                _redisStore.Delete(tenantType);

                if (SecondaryLockHandler != null)
                {
                    Task.Factory.StartNew(() => SecondaryLockHandler.Release(tenantType));
                }
            }
            catch (Exception ex)
            {
                SecondaryLockHandler?.Release(tenantType);
            }
        }
    }
}