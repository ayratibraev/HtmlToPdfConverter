using CommonUtils.Services.Interfaces;
using MediatR;

namespace Uploader.Application.Notifications;

public sealed class HtmlUploadedNotification : INotification
{
    public string FilePath { get; }

    public HtmlUploadedNotification(string filePath)
    {
        FilePath = filePath;
    }
}

public sealed class HtmlUploadedHandler : INotificationHandler<HtmlUploadedNotification>
{
    private readonly IStorage _storage;

    public HtmlUploadedHandler(IStorage storage)
    {
        _storage = storage;
    }

    public Task Handle(HtmlUploadedNotification notification, CancellationToken cancellationToken)
    {
        _storage.Upload(notification.FilePath);
        return Task.CompletedTask;
    }
}
