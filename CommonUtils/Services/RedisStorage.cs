using CommonUtils.Application.LogEvent;
using CommonUtils.Services.Interfaces;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace CommonUtils.Services;

public sealed class RedisStorage : IStorage
{
    private readonly string _connectionString;
    private readonly IFileSystem _fileSystem;
    private readonly ILogger<RedisStorage> _logger;
    private IDatabase _db;

    private static string KeyHtml => "html_file";
    private static string KeyPdf => "pdf_file";

    public RedisStorage(string connectionString, IFileSystem fileSystem, ILogger<RedisStorage> logger)
    {
        _connectionString = connectionString;
        _fileSystem = fileSystem;
        _logger = logger;
        
        ConnectRedis();
    }
    
    private static ConnectionMultiplexer _redis;
    private static readonly Object _multiplexerLock = new Object();

    private void ConnectRedis()
    {
        try
        {
            _logger.LogInformation("Try connect redis: {_connectionString}", _connectionString);
            _redis = ConnectionMultiplexer.Connect(_connectionString);
        }
        catch (Exception ex)
        {
            //exception handling goes here
        }
    }


    private ConnectionMultiplexer RedisMultiplexer
    {
        get
        {
            lock (_multiplexerLock)
            {
                if (_redis == null || !_redis.IsConnected)
                {
                    ConnectRedis();
                }
                return _redis;
            }
        }
    }

    private void Upload(string filePath, string key)
    {
        try
        {
            var db = RedisMultiplexer.GetDatabase();
            db.StringSet(key, File.ReadAllBytes(filePath));
            _fileSystem.DeleteFile(filePath);
            var html = db.StringGet(key);
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
            var db = RedisMultiplexer.GetDatabase();
            var tran = db.CreateTransaction();
            var getResult = tran.StringGetAsync(key);
            tran.KeyDeleteAsync(key);
            tran.Execute();
            var value = getResult.Result;

            if (!value.HasValue)
            {
                return null;
            };

            var path = 
                key == KeyHtml 
                    ? _fileSystem.GetTempFileNameHtml()
                    : _fileSystem.GetTempFileNamePdf();
            _logger.LogInformation($"File {key} downloaded to: {path}");
            File.WriteAllBytes(path, value);
            return path;
        }
        catch (Exception exception)
        {
            // _logger.LogError(RedisLogEvent.Download,
            //     exception,
            //     "RedisStorage.Download failed.key: {key}", 
            //     key);
            //
            // throw;
            
            return null;
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