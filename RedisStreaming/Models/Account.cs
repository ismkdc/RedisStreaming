namespace RedisStreaming.Models;

public class Account
{
    public int Seq { get; set; }
    public Guid Id { get; set; }
    public string Name { get; set; }
}