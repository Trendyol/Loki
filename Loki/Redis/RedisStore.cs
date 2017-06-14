using System;
using System.Net;
using StackExchange.Redis;

namespace Loki.Redis
{
    public class RedisStore : IRedisStore
    {
        private static readonly Lazy<RedisStore> _Instance = new Lazy<RedisStore>(() => new RedisStore());
        private static IDatabase _database;

        public static RedisStore Instance => _Instance.Value;
        public RedisStore Initialize(EndPoint[] redisEndPoints)
        {
            _database = RedisConnectionFactory.Instance.SetEndPoints(redisEndPoints)
                .GetConnection().GetDatabase(0);

            return this;
        }

        public string Get(string key)
        {
            return _database.StringGet(key);
        }

        public bool Set(string key, string value, int expiryFromSeconds)
        {
            bool result = _database.LockTake(key, value, TimeSpan.FromSeconds(expiryFromSeconds));
            return result;
        }

        public void Delete(string key, string value)
        {
            _database.LockRelease(key, value);
        }
    }
}