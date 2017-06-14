using System.Net;

namespace Loki.Redis
{
    interface IRedisStore
    {
        RedisStore Initialize(EndPoint[] redisEndPoints);
        string Get(string key);
        bool Set(string key, string value, int expiryFromSeconds);
        void Delete(string key, string value);
    }
}