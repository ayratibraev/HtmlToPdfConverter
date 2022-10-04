using StackExchange.Redis;
using Uploader.Application.Services.Interfaces;

namespace Uploader.Application.Services;

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
        var pdf = _db.StringGet("html_file");

        if (pdf.HasValue)
        {
            var path = Path.GetTempFileName();
            File.WriteAllText(path, pdf.ToString());
            return path;
        }

        return null;
    }
}