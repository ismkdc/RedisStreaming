using Bogus;
using Microsoft.AspNetCore.Mvc;
using RedisStreaming.Models;
using RedisStreaming.Producers;

namespace RedisStreaming.Controllers;

[ApiController]
[Route("accounts")]
public class AccountController(AccountCreateProducer producer) : ControllerBase
{
    [HttpPost("create-1-million")]
    public async Task<IActionResult> Create1MillionAccounts()
    {
        var faker = new Faker<Account>();
        var accounts = faker
            .RuleFor(x => x.Seq, f => f.IndexFaker)
            .RuleFor(x => x.Id, f => f.Random.Guid())
            .RuleFor(x => x.Name, f => f.Person.FullName)
            .Generate(1_000_000);

        var tasks = accounts.Select(producer.ProduceAsync);
        await Task.WhenAll(tasks);

        return Ok("Check app console for progress :]");
    }
}