using System;
using System.Net;
using RedLock;

namespace Loki
{
    public class RedisLokiLockHandler : LokiLockHandler
    {
        private static RedisLockFactory _redisLockFactory;
        private IRedisLock _redisLock;

        public RedisLokiLockHandler(EndPoint[] redisEndPoints)
        {
            _redisLockFactory = new RedisLockFactory(redisEndPoints);
        }

        public override bool Lock(string tenantType, int expiryFromSeconds)
        {
            _redisLock = _redisLockFactory.Create(tenantType, TimeSpan.FromSeconds(expiryFromSeconds), TimeSpan.FromSeconds(expiryFromSeconds * 2), TimeSpan.FromSeconds(1));

            if (!_redisLock.IsAcquired && SecondaryLockHandler != null)
            {
                return SecondaryLockHandler.Lock(tenantType, expiryFromSeconds);
            }

            return _redisLock.IsAcquired;
        }

        public override void Release(string tenantType)
        {
            _redisLock.Dispose();
        }
    }
}