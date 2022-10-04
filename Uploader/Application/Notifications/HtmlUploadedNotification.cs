using MediatR;
using StackExchange.Redis;

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
    public Task Handle(HtmlUploadedNotification notification, CancellationToken cancellationToken)
    {
        var redis = ConnectionMultiplexer.Connect("localhost");
        var db = redis.GetDatabase();

        var value = System.IO.File.ReadAllBytes(notification.FilePath);
        db.StringSet("html_file", value);

        return Task.CompletedTask;
    }
}
