using System;

using StackExchange.Redis;

namespace Ouch1978.Wpf.RedisKeyFilterSample
{
    public static class RedisConnectionFactory
    {
        private static readonly Lazy<ConnectionMultiplexer> Connection;
        static RedisConnectionFactory()
        {
            var connectionString = "127.0.0.1:6379";
            var options = ConfigurationOptions.Parse(connectionString);
            Connection = new Lazy<ConnectionMultiplexer>(() => ConnectionMultiplexer.Connect(options));
        }
        public static ConnectionMultiplexer GetConnection => Connection.Value;
        public static IDatabase RedisDb => GetConnection.GetDatabase();
    }
}
