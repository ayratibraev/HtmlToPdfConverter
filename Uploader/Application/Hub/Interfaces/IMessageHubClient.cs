namespace Uploader.Application.Hub.Interfaces;

public interface IMessageHubClient
{
    Task SendPdfReady(string fileName);
}