using MediatR;
using StackExchange.Redis;

namespace Uploader.Application.Notifications;

public class FileUploadedNotification : INotification
{
    public string FilePath { get; }

    public FileUploadedNotification(string filePath)
    {
        FilePath = filePath;
    }
}

public class FileUploadedHandler : INotificationHandler<FileUploadedNotification>
{
    public async Task Handle(FileUploadedNotification notification, CancellationToken cancellationToken)
    {
        var redis = await ConnectionMultiplexer.ConnectAsync("localhost");
        var db = redis.GetDatabase();

        var value = System.IO.File.ReadAllBytes(notification.FilePath);
        db.StringSet("html_file", value);
    }
}
