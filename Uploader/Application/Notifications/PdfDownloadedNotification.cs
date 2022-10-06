using MediatR;
using Microsoft.AspNetCore.SignalR;
using Uploader.Application.Hub;
using Uploader.Application.Hub.Interfaces;

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
    private readonly IHubContext<MessageHub, IMessageHubClient> _hubContext;

    public PdfDownloadedHandler(ILogger<PdfDownloadedHandler> logger, IHubContext<MessageHub, IMessageHubClient> hubContext)
    {
        _logger = logger;
        _hubContext = hubContext;
    }

    public Task Handle(PdfDownloadedNotification notification, CancellationToken cancellationToken)
    {
        // TODO: Вынести в нормальное место
        _hubContext.Clients.All.SendPdfReady(
            "https://localhost:7123/api/files/download?fileName="
            + Path.GetFileName(notification.FilePath));
        return Task.CompletedTask;
    }
}