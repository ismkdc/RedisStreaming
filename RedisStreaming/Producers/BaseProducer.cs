using System.Text.Json;
using StackExchange.Redis;

namespace RedisStreaming.Producers;

public abstract class BaseProducer<T>(IDatabase redisDatabase, string streamName)
{
    public Task ProduceAsync(T message)
    {
        return redisDatabase.StreamAddAsync(streamName, string.Empty, JsonSerializer.Serialize(message));
    }
}