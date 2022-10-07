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
    private readonly IConfiguration _configuration;

    public PdfDownloadedHandler(ILogger<PdfDownloadedHandler> logger, 
            IHubContext<MessageHub, IMessageHubClient> hubContext,
            IConfiguration configuration)
    {
        _logger = logger;
        _hubContext = hubContext;
        _configuration = configuration;
    }

    public Task Handle(PdfDownloadedNotification notification, CancellationToken cancellationToken)
    {
        _hubContext.Clients.All.SendPdfReady(
            _configuration.GetValue<string>("uploadPdfUrl")
            + Path.GetFileName(notification.FilePath));
        return Task.CompletedTask;
    }
}