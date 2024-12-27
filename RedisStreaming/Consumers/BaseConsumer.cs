using StackExchange.Redis;

namespace RedisStreaming.Consumers;

public abstract class BaseConsumer(IDatabase redisDatabase) : BackgroundService
{
    private readonly string _consumerName = Guid.NewGuid().ToString();
    protected abstract int BatchCount { get; }
    protected abstract string StreamName { get; }
    protected abstract string GroupName { get; }

    protected async Task Consume
    (
        CancellationToken stoppingToken
    )
    {
        while (!stoppingToken.IsCancellationRequested)
            try
            {
                if (!await redisDatabase.KeyExistsAsync(StreamName) ||
                    (await redisDatabase.StreamGroupInfoAsync(StreamName)).All(x => x.Name != GroupName)
                   )
                    await redisDatabase.StreamCreateConsumerGroupAsync(
                        StreamName,
                        GroupName,
                        StreamPosition.Beginning
                    );

                await Task.Delay(500, stoppingToken);

                while (!stoppingToken.IsCancellationRequested)
                    try
                    {
                        var claimed = await ClaimPendingMessages();
                        if (claimed.ClaimedEntries.Length > 0)
                            await ReadClaimedMessages();

                        await ReadNewMessages();
                        await Task.Delay(100, stoppingToken);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }

                await Task.Delay(5000, stoppingToken);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
    }

    private async Task ReadClaimedMessages()
    {
        var streamResult =
            await redisDatabase.StreamReadGroupAsync
            (
                StreamName,
                GroupName,
                _consumerName,
                StreamPosition.Beginning,
                BatchCount
            );

        if (streamResult.Length > 0) await HandleMessages(streamResult);
    }

    private async Task ReadNewMessages()
    {
        var streamResult =
            await redisDatabase.StreamReadGroupAsync
            (
                StreamName,
                GroupName,
                _consumerName,
                ">",
                BatchCount
            );

        if (streamResult.Length > 0) await HandleMessages(streamResult);
    }

    private async Task HandleMessages(IEnumerable<StreamEntry> streamResult)
    {
        foreach (var data in streamResult)
        foreach (var messageItem in data.Values)
            try
            {
                await OnMessage(messageItem, data.Id);
                await redisDatabase.StreamAcknowledgeAsync(StreamName, GroupName, data.Id);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
    }

    private Task<StreamAutoClaimResult> ClaimPendingMessages()
    {
        return redisDatabase.StreamAutoClaimAsync
        (
            StreamName,
            GroupName,
            _consumerName,
            TimeSpan
                .FromMinutes(10)
                .Milliseconds,
            StreamPosition.Beginning,
            BatchCount
        );
    }

    protected abstract Task OnMessage(NameValueEntry message, RedisValue messageId);
}