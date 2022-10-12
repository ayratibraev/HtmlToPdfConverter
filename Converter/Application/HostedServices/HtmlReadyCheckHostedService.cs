using CommonUtils.Services.Interfaces;
using Converter.Application.Notifications;
using MediatR;

namespace Converter.Application.HostedServices;

public sealed class HtmlReadyCheckHostedService: IHostedService, IDisposable
{
    private readonly ILogger<HtmlReadyCheckHostedService> _logger;
    private readonly IStorage _storage;
    private readonly IMediator _mediatr;
    private Timer? _timer = null;

    public HtmlReadyCheckHostedService(ILogger<HtmlReadyCheckHostedService> logger, IStorage storage, IMediator mediatr)
    {
        _logger = logger;
        _storage = storage;
        _mediatr = mediatr;
    }

    public Task StartAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("HtmlReadyCheckHostedService running.");
        
        _timer = new Timer(DoWork, null, TimeSpan.Zero,
            TimeSpan.FromSeconds(5));
        
        return Task.CompletedTask;
    }

    private void DoWork(object? state)
    {
        try
        {
            var filePath = _storage.DownloadHtml();
            if (string.IsNullOrEmpty(filePath) == false)
            {
                _mediatr.Publish(new HtmlUploadedNotification(filePath));
            }
            
        }
        catch (Exception exception)
        {
            _logger.LogInformation("Exception. {0}", exception);
        }
    }

    public Task StopAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("HtmlReadyCheckHostedService is stopping.");
        _timer?.Change(Timeout.Infinite, 0);
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _timer?.Dispose();
    }
}