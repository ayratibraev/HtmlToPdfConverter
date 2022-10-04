using MediatR;
using StackExchange.Redis;
using Uploader.Application.Notifications;
using Uploader.Application.Services.Interfaces;

namespace Uploader.Application.HostedServices;

public sealed class PdfReadyCheckHostedService: IHostedService, IDisposable
{
    private int executionCount = 0;
    private readonly ILogger<PdfReadyCheckHostedService> _logger;
    private readonly IStorage _storage;
    private readonly IMediator _mediatr;
    private Timer? _timer = null;

    public PdfReadyCheckHostedService(ILogger<PdfReadyCheckHostedService> logger, IStorage storage, IMediator mediatr)
    {
        _logger = logger;
        _storage = storage;
        _mediatr = mediatr;
    }

    public Task StartAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Timed Hosted Service running.");

        _timer = new Timer(DoWork, null, TimeSpan.Zero,
            TimeSpan.FromSeconds(2));

        return Task.CompletedTask;
    }

    private void DoWork(object? state)
    {
        var count = Interlocked.Increment(ref executionCount);

        var filePath = _storage.Download();
        if (string.IsNullOrEmpty(filePath) == false)
        {
            _mediatr.Publish(new PdfDownloadedNotification(filePath));   
        }
        
        _logger.LogInformation(
            "Timed Hosted Service is working. Count: {Count}", count);
    }

    public Task StopAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Timed Hosted Service is stopping.");

        _timer?.Change(Timeout.Infinite, 0);

        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _timer?.Dispose();
    }
}