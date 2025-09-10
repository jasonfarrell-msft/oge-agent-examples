
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace Farrellsoft.Examples.Agents.MultiAgent.Services
{
    public class RedisCacheService(ILogger<RedisCacheService> logger, IConfiguration configuration) : ICacheService
    {
        public async Task WriteAsync<T>(string key, T value)
        {
            var connectionString = configuration["RedisConnectionString"]!;

            var redis = await ConnectionMultiplexer.ConnectAsync(connectionString);
            logger.LogInformation("Connected to Redis");
            try
            {
                var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
                var jsonValue = JsonSerializer.Serialize(value, options);

                var database = redis.GetDatabase();
                await database.StringSetAsync(key, jsonValue);
                logger.LogInformation("Wrote key {Key} to Redis", key);
            }
            finally
            {
                redis.Dispose();
                logger.LogInformation("Disconnected from Redis");
            }
        }
    }
}

public interface ICacheService
{
    Task WriteAsync<T>(string key, T value);
}
