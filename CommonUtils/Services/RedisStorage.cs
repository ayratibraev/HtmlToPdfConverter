using System.Text;
using CommonUtils.Services.Interfaces;
using StackExchange.Redis;

namespace CommonUtils.Services;

public sealed class RedisStorage : IStorage
{
    private readonly IFileSystem _fileSystem;
    private readonly IDatabase _db;

    private static string KeyHtml => "html_file";
    private static string KeyPdf => "pdf_file";

    public RedisStorage(string host, int port, IFileSystem fileSystem)
    {
        _fileSystem = fileSystem;

        var config = new ConfigurationOptions()
        {
            KeepAlive = 0,
            AllowAdmin = true,
            EndPoints = { { host, port } },
            ConnectTimeout = 5000,
            ConnectRetry = 5,
            SyncTimeout = 5000,
            AbortOnConnectFail = false,
        };
        
        var redis = ConnectionMultiplexer.Connect(config);
        _db = redis.GetDatabase();
    }

    private void Upload(string filePath, string key)
    {
        _db.StringSet(key, File.ReadAllBytes(filePath));
        _fileSystem.DeleteFile(filePath);
    }

    private string? Download(string key)
    {
        var tran = _db.CreateTransaction();
        var getResult = tran.StringGetAsync(key);
        tran.KeyDeleteAsync(key);
        tran.Execute();
        var value = getResult.Result;

        if (!value.HasValue) return null;

        var path = 
            key == KeyHtml 
                ? _fileSystem.GetTempFileNameHtml()
                : _fileSystem.GetTempFileNamePdf();
        
        File.WriteAllBytes(path, value);
        return path;
    }

    public void UploadHtml(string filePath)
    {
        Upload(filePath, KeyHtml);
    }

    public string? DownloadHtml()
    {
        return Download(KeyHtml);
    }

    public void UploadPdf(string filePath)
    {
        Upload(filePath, KeyPdf);
    }

    public string? DownloadPdf()
    {
        return Download(KeyPdf);
    }
}