using System.Collections.Concurrent;
using System.Security.Claims;
using System.Text.Json;
using AudioAnalyzer.Data;
using AudioAnalyzer.Data.Persistence.Models;
using AudioAnalyzer.Data.Persistence.Repositories;
using Microsoft.AspNetCore.SignalR;

namespace AudioAnalyzer.Web.Hubs;

public class FileUploadHub : Hub
{
    // Store connection IDs with corresponding client identifiers (like file request ID)
    private readonly FileUploadHubConnectionContext _connectionContext;
    private readonly DatabaseService _databaseService;
    public FileUploadHub(FileUploadHubConnectionContext connectionConnectionContext,
                         DatabaseService databaseService)
    {
        _connectionContext = connectionConnectionContext;
        _databaseService = databaseService;
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

        return await _databaseService.UserRepository.GetEntity(
            id:userId, 
            includeRelatedEntities: true);
    }
}

public static class FileHubMethodNames
{
    public const string FileTextForSearch = "TranscribedTextForSearch";
    public const string FileText = "TranscribedText";
}
