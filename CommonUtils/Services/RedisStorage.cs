using CommonUtils.Services.Interfaces;
using StackExchange.Redis;

namespace CommonUtils.Services;

public class RedisStorage : IStorage
{
    private readonly IDatabase _db;

    public RedisStorage()
    {
        var redis = ConnectionMultiplexer.Connect("localhost");
        _db = redis.GetDatabase();
    }

    public void Upload(string filePath)
    {
        _db.StringSet("html_file", File.ReadAllBytes(filePath));
    }

    public string? Download()
    {
        var tran = _db.CreateTransaction();
        var getResult = tran.StringGetAsync("html_file");
        tran.KeyDeleteAsync("html_file");
        tran.Execute();
        var value = getResult.Result;

        if (!value.HasValue) return null;
        
        var path = Path.GetTempFileName();
        File.WriteAllText(path, value.ToString());
        return path;
    }
}