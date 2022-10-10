using CommonUtils.Application.LogEvent;
using CommonUtils.Services.Interfaces;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace CommonUtils.Services;

public sealed class RedisStorage : IStorage
{
    private readonly IFileSystem _fileSystem;
    private readonly ILogger<RedisStorage> _logger;
    private IDatabase _db;

    private static string KeyHtml => "html_file";
    private static string KeyPdf => "pdf_file";

    public RedisStorage(string connectionString, IFileSystem fileSystem, ILogger<RedisStorage> logger)
    {
        _fileSystem = fileSystem;
        _logger = logger;
        
        Connect(connectionString);
    }

    private void Connect(string connectionString)
    {
        try
        {
            var redis = ConnectionMultiplexer.Connect(connectionString);
            _db = redis.GetDatabase();
        }
        catch (Exception exception)
        {
            _logger.LogError(RedisLogEvent.Upload,
                exception,
                "RedisStorage.Connect failed. connectionString: {connectionString}", 
                connectionString);
            
            throw;
        }
    }

    private void Upload(string filePath, string key)
    {
        try
        {
            _db.StringSet(key, File.ReadAllBytes(filePath));
            _fileSystem.DeleteFile(filePath);
        }
        catch (Exception exception)
        {
            _logger.LogError(RedisLogEvent.Upload,
                exception,
                "RedisStorage.Upload failed. filepath: {filepath}, key: {key}", 
                filePath, key);
            
            throw;
        }
    }

    private string? Download(string key)
    {
        try
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
        catch (Exception exception)
        {
            _logger.LogError(RedisLogEvent.Download,
                exception,
                "RedisStorage.Download failed.key: {key}", 
                key);

            throw;
        }
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