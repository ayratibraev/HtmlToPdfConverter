using MediatR;
using StackExchange.Redis;
using Uploader.Application.Notifications;

namespace Uploader.Application.HostedServices;

public sealed class PdfReadyCheckHostedService: IHostedService, IDisposable
{
    private int executionCount = 0;
    private readonly ILogger<PdfReadyCheckHostedService> _logger;
    private readonly IMediator _mediatr;
    private Timer? _timer = null;
    private readonly ConnectionMultiplexer _redis;

    public PdfReadyCheckHostedService(ILogger<PdfReadyCheckHostedService> logger, IMediator mediatr)
    {
        _logger = logger;
        _mediatr = mediatr;
        _redis = ConnectionMultiplexer.Connect("localhost");
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
        
        var db = _redis.GetDatabase();
        var pdf = db.StringGet("html_file");

        if (pdf.HasValue)
        {
            var path = Path.GetTempFileName();
            File.WriteAllText(path, pdf.ToString());
            _mediatr.Publish(new PdfDownloadedNotification(path));
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