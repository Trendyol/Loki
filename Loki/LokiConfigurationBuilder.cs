using System;

namespace Loki
{
    public class LokiConfigurationBuilder
    {
        private static readonly Lazy<LokiConfigurationBuilder> _Instance = new Lazy<LokiConfigurationBuilder>(() => new LokiConfigurationBuilder());

        public static LokiConfigurationBuilder Instance => _Instance.Value;

        private LokiConfigurationBuilder()
        {

        }

        public LokiConfigurationBuilder SetPrimaryLockHandler(LokiLockHandler lokiLockHandler)
        {
            LokiConfiguration.LokiLockHandler = lokiLockHandler;

            return this;
        }

        public LokiConfigurationBuilder SetSecondaryLockHandler(LokiLockHandler lokiLockHandler)
        {
            LokiConfiguration.LokiLockHandler.SetHandler(lokiLockHandler);

            return this;
        }

        public LokiConfigurationBuilder SetServiceKey(string serviceKey)
        {
            LokiConfiguration.ServiceKey = serviceKey;

            return this;
        }

        public LokiConfigurationBuilder Build()
        {
            return this;
        }
    }
}