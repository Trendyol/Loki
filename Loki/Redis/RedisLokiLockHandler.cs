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

        public override bool Lock(string serviceKey, int expiryFromSeconds)
        {
            bool isLocked = false;

            try
            {
                string isAnyLocked = _redisStore.Get(serviceKey);

                if (String.IsNullOrEmpty(isAnyLocked))
                {
                    isLocked = _redisStore.Set(serviceKey, "locked", expiryFromSeconds);

                    // When occurs any network problem within Redis, it could be provided consistent locking with secondary handler.
                    if (SecondaryLockHandler != null)
                    {
                        Task.Factory.StartNew(() => SecondaryLockHandler.Lock(serviceKey, expiryFromSeconds));
                    }
                }
            }
            catch (Exception ex)
            {
                if (SecondaryLockHandler != null)
                {
                    isLocked = SecondaryLockHandler.Lock(serviceKey, expiryFromSeconds);
                }
            }

            return isLocked;
        }

        public override void Release(string serviceKey)
        {
            try
            {
                _redisStore.Delete(serviceKey);

                if (SecondaryLockHandler != null)
                {
                    Task.Factory.StartNew(() => SecondaryLockHandler.Release(serviceKey));
                }
            }
            catch (Exception ex)
            {
                SecondaryLockHandler?.Release(serviceKey);
            }
        }
    }
}