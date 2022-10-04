using MediatR;

namespace Uploader.Application.Notifications;

public sealed class PdfDownloadedNotification : INotification
{
    public string FilePath { get; }

    public PdfDownloadedNotification(string filePath)
    {
        FilePath = filePath;
    }
}

public sealed class PdfDownloadedHandler : INotificationHandler<PdfDownloadedNotification>
{
    private readonly ILogger<PdfDownloadedHandler> _logger;

    public PdfDownloadedHandler(ILogger<PdfDownloadedHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(PdfDownloadedNotification notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Pdf downloaded in: {notification.FilePath}");
        return Task.CompletedTask;
    }
}