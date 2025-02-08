using System.Collections.Concurrent;
using AudioAnalyzer.Web.Models;
using Microsoft.AspNetCore.SignalR;

namespace AudioAnalyzer.Web.Hubs;

public class FileUploadHub : Hub
{
    // Store connection IDs with corresponding client identifiers (like file request ID)
    private static readonly ConcurrentDictionary<string, User> _connections = new();

    // Associate a connection ID with a specific user/file request ID
    public async Task RegisterClient(string fileRequestId)
    {

    }

    // Notify a specific client when the file processing is done
    public async Task SendFileProcessedMessage(string fileRequestId, string message)
    {
        if (_connections.TryGetValue(fileRequestId, out var user))
        {
            await Clients.Client(user.ConnectionId).SendAsync("ReceiveMessage", message);
        }
    }

    public override async Task OnConnectedAsync()
    {
        _connections.TryAdd(Context.ConnectionId, new User
        {
            ConnectionId = Context.ConnectionId
        });
        
        await base.OnConnectedAsync();
    }

    // Optionally, allow clients to unregister or disconnect
    public override async Task OnDisconnectedAsync(Exception exception)
    {
        // Clean up connection when client disconnects
        
        _connections.TryRemove(Context.ConnectionId, out _);
        
        await base.OnDisconnectedAsync(exception);
    }
}
