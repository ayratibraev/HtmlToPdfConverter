using System.Runtime.InteropServices;
using CommonUtils.Services.Interfaces;
using Converter.Application.Services.Interfaces;
using MediatR;
using PuppeteerSharp;
using StackExchange.Redis;

namespace Converter.Application.Notifications;

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
    private readonly IMediator _mediator;
    private readonly IPdfConverter _pdfConverter;

    public HtmlUploadedHandler(IMediator mediator, IPdfConverter pdfConverter)
    {
        _mediator = mediator;
        _pdfConverter = pdfConverter;
    }

    public async Task Handle(HtmlUploadedNotification notification, CancellationToken cancellationToken)
    {
        var pdfPath = await _pdfConverter.Convert(notification.FilePath);
        await _mediator.Publish(new PdfIsReadyNotification(pdfPath), cancellationToken);
    }
}
