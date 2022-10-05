using System.Text;
using CommonUtils.Services.Interfaces;
using StackExchange.Redis;

namespace CommonUtils.Services;

public sealed class RedisStorage : IStorage
{
    private readonly IDatabase _db;

    public RedisStorage()
    {
        var redis = ConnectionMultiplexer.Connect("localhost");
        _db = redis.GetDatabase();
    }

    private void Upload(string filePath, string key)
    {
        _db.StringSet(key, File.ReadAllBytes(filePath));
    }

    private string? Download(string key)
    {
        var tran = _db.CreateTransaction();
        var getResult = tran.StringGetAsync(key);
        tran.KeyDeleteAsync(key);
        tran.Execute();
        var value = getResult.Result;

        if (!value.HasValue) return null;
        
        var path = Path.GetTempFileName();
        File.WriteAllBytes(path, new UTF8Encoding().GetBytes(value.ToString()));
        return path;
    }

    public void UploadHtml(string filePath)
    {
        Upload(filePath, "html_file");
    }

    public string? DownloadHtml()
    {
        return Download("html_file");
    }

    public void UploadPdf(string filePath)
    {
        Upload(filePath, "pdf_file");
    }

    public string? DownloadPdf()
    {
        return Download("pdf_file");
    }
}