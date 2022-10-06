using Microsoft.AspNetCore.SignalR;
using Uploader.Application.Hub.Interfaces;

namespace Uploader.Application.Hub;

public class MessageHub : Hub<IMessageHubClient>
{
    public async Task SendPdfReady(string fileName)
    {
        await Clients.All.SendPdfReady(fileName);
    }
}