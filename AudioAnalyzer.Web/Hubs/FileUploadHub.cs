using System.Collections.Concurrent;
using System.Security.Claims;
using System.Text.Json;
using AudioAnalyzer.Data;
using AudioAnalyzer.Data.Models;
using Microsoft.AspNetCore.SignalR;

namespace AudioAnalyzer.Web.Hubs;

public class FileUploadHub : Hub
{
    // Store connection IDs with corresponding client identifiers (like file request ID)
    private readonly FileUploadHubConnectionContext _connectionContext;
    private readonly DatabaseDbContextService _databaseDbContextService;
    public FileUploadHub(FileUploadHubConnectionContext connectionConnectionContext,
                         DatabaseDbContextService databaseDbContextService)
    {
        _connectionContext = connectionConnectionContext;
        _databaseDbContextService = databaseDbContextService;
    }

    // Notify a specific client when the file processing is done

    public override async Task OnConnectedAsync()
    {
        var user = await GetUserFromDb();
        
        if (user == null)
            return;
        
        user.ConnectionId = Context.ConnectionId;
        _connectionContext.ActiveUsers.TryAdd(user, Byte.MinValue);
        await base.OnConnectedAsync();
    }

    // Optionally, allow clients to unregister or disconnect
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        // Clean up connection when client disconnects
        var user = GetUserFromActiveConnectionsByContext();
        
        if (user != null)
        {
            _connectionContext.ActiveUsers.Remove(user, out _);
        }
        
        await base.OnDisconnectedAsync(exception);
    }

    private User? GetUserFromActiveConnectionsByContext()
    {
        return _connectionContext.ActiveUsers.Keys.FirstOrDefault(u => u.ConnectionId == Context.ConnectionId);
    }

    private async Task<User?> GetUserFromDb()
    {
        var userNameId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        
        if (!Int32.TryParse(userNameId, out var userId))
            return null;

        return await _databaseDbContextService.UserRepository.GetEntity(
            id:userId, 
            includeRelatedEntities: true);
    }
}

public static class FileHubMethodNames
{
    public const string FileTextForSearch = "TranscribedTextForSearch";
    public const string FileText = "TranscribedText";
}
