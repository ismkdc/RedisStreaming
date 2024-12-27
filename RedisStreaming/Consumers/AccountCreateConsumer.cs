using System.Text.Json;
using RedisStreaming.Constants;
using RedisStreaming.Models;
using StackExchange.Redis;

namespace RedisStreaming.Consumers;

public class AccountCreateConsumer(
    IDatabase redisDatabase
)
    : BaseConsumer(redisDatabase)
{
    protected override int BatchCount { get; } = 100;
    protected override string StreamName { get; } = StreamNames.AccountCreate;
    protected override string GroupName { get; } = $"{StreamNames.AccountCreate}_CONSUMER_GROUP";


    protected override async Task OnMessage(NameValueEntry message, RedisValue messageId)
    {
        var user = JsonSerializer.Deserialize<Account>(message.Value);

        Console.WriteLine(JsonSerializer.Serialize(user));
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Consume(stoppingToken);
    }
}