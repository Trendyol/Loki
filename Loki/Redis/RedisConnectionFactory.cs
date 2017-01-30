using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using StackExchange.Redis;

namespace Loki.Redis
{
    public class RedisConnectionFactory
    {
        private static readonly Lazy<RedisConnectionFactory> _Instance = new Lazy<RedisConnectionFactory>(() => new RedisConnectionFactory());
        private static Lazy<ConnectionMultiplexer> _connection;
        private static List<EndPoint> _endPoints;

        public static RedisConnectionFactory Instance => _Instance.Value;

        public RedisConnectionFactory SetEndPoints(EndPoint[] redisEndPoints)
        {
            if (!redisEndPoints.Any())
            {
                throw new ArgumentException("No endpoints found!");
            }

            _endPoints = new List<EndPoint>(redisEndPoints.Length);
            _endPoints.AddRange(redisEndPoints.ToArray());

            return this;
        }

        public ConnectionMultiplexer GetConnection()
        {
            var configuration = new ConfigurationOptions
            {
                AbortOnConnectFail = false
            };

            foreach (var endPoint in _endPoints)
            {
                configuration.EndPoints.Add(endPoint);
            }

            _connection = new Lazy<ConnectionMultiplexer>(() => ConnectionMultiplexer.Connect(configuration));

            return _connection.Value;
        }
    }
}