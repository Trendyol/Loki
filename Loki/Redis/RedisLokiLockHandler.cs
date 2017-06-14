using System;
using System.Net;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace Loki.Redis
{
    public class RedisLokiLockHandler : LokiLockHandler
    {
        private static IRedisStore _redisStore;
        private static RedisValue _token;

        public RedisLokiLockHandler(EndPoint[] redisEndPoints)
        {
            _redisStore = RedisStore.Instance.Initialize(redisEndPoints);
            _token = Environment.MachineName;
        }

        public override bool Lock(string serviceKey, int expiryFromSeconds)
        {
            bool isLocked = false;

            try
            {
                string isAnyLocked = _redisStore.Get(serviceKey);

                if (String.IsNullOrEmpty(isAnyLocked))
                {
                    isLocked = _redisStore.Set(serviceKey, _token, expiryFromSeconds);

                    // When occurs any network problem within Redis, it could be provided consistent locking with secondary handler.
                    if (SecondaryLockHandler != null)
                    {
                        Task.Factory.StartNew(() => SecondaryLockHandler.Lock(serviceKey, expiryFromSeconds));
                    }
                }
            }
            catch
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
                _redisStore.Delete(serviceKey, _token);

                if (SecondaryLockHandler != null)
                {
                    Task.Factory.StartNew(() => SecondaryLockHandler.Release(serviceKey));
                }
            }
            catch
            {
                SecondaryLockHandler?.Release(serviceKey);
            }
        }
    }
}