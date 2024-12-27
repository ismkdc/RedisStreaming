using RedisStreaming.Constants;
using RedisStreaming.Models;
using StackExchange.Redis;

namespace RedisStreaming.Producers;

public class AccountCreateProducer(IDatabase redisDatabase)
    : BaseProducer<Account>(redisDatabase, StreamNames.AccountCreate)
{
}