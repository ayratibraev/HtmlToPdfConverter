using Converter.Application.Services.Interfaces;
using MediatR;

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
    private readonly ILogger<HtmlUploadedHandler> _logger;

    public HtmlUploadedHandler(IMediator mediator, IPdfConverter pdfConverter, ILogger<HtmlUploadedHandler> logger)
    {
        _mediator = mediator;
        _pdfConverter = pdfConverter;
        this._logger = logger;
    }

    public async Task Handle(HtmlUploadedNotification notification, CancellationToken cancellationToken)
    {
        var pdfPath = await _pdfConverter.Convert(notification.FilePath);
        await _mediator.Publish(new PdfIsReadyNotification(pdfPath), cancellationToken);
    }
}
