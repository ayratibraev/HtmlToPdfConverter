using CommonUtils.Services.Interfaces;
using MediatR;

namespace Converter.Application.Notifications;

public sealed class PdfIsReadyNotification : INotification
{
    public string PdfPath { get; }

    public PdfIsReadyNotification(string pdfPath)
    {
        PdfPath = pdfPath;
    }
}

public sealed class PdfIsReadyHandler : INotificationHandler<PdfIsReadyNotification>
{
    private readonly IStorage _storage;

    public PdfIsReadyHandler(IStorage storage)
    {
        _storage = storage;
    }

    public Task Handle(PdfIsReadyNotification notification, CancellationToken cancellationToken)
    {
        _storage.UploadPdf(notification.PdfPath);
        return Task.CompletedTask;
    }
}