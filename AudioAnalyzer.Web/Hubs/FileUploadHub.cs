using System.Collections.Concurrent;
using System.Security.Claims;
using System.Text.Json;
using AudioAnalyzer.Data.Persistence.Models;
using AudioAnalyzer.Data.Persistence.Repositories;
using AudioAnalyzer.Web.Models.AudioTranscribeResponse;
using Microsoft.AspNetCore.SignalR;

namespace AudioAnalyzer.Web.Hubs;

public class FileUploadHub : Hub
{
    IRepository<User> _userRepository;
    // Store connection IDs with corresponding client identifiers (like file request ID)
    private readonly ConcurrentDictionary<User, byte> _activeUsers;
    public FileUploadHub(IRepository<User> userRepository)
    {
        _userRepository = userRepository;
        _activeUsers = new ConcurrentDictionary<User, byte>();
    }

    // Notify a specific client when the file processing is done
    public async Task SendTranscribedText(int userId, string text)
    {
        var user = GetUserFromActiveConnections(userId);
        
        if (user == null)
            return;
        
        await Clients.Client(user.ConnectionId).SendAsync(FileHubMethodNames.FileText, text);
    }


    public async Task SendTranscribedTextForSearch(int userId, TranscribedResponseJson transcribedTextJson)
    {
        var user = GetUserFromActiveConnections(userId);
        
        if (user == null)
            return;
        
        var text = JsonSerializer.Serialize(transcribedTextJson);

        await Clients.Client(user.ConnectionId).SendAsync(FileHubMethodNames.FileTextForSearch, text);
    }

    public override async Task OnConnectedAsync()
    {
        var user = GetUserFromDb();
        
        if (user == null)
            return;
        
        user.ConnectionId = Context.ConnectionId;
        _activeUsers.TryAdd(user, Byte.MinValue);
        await base.OnConnectedAsync();
    }

    // Optionally, allow clients to unregister or disconnect
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        // Clean up connection when client disconnects
        var user = GetUserFromActiveConnectionsByContext();
        
        if (user != null)
        {
            _activeUsers.Remove(user, out _);
        }
        
        await base.OnDisconnectedAsync(exception);
    }

    private User? GetUserFromActiveConnectionsByContext()
    {
        return _activeUsers.Keys.FirstOrDefault(u => u.ConnectionId == Context.ConnectionId);
    }

    private User? GetUserFromDb()
    {
        var userNameId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        
        if (!Int32.TryParse(userNameId, out var userId))
            return null;

        return _userRepository.GetEntity(
            id:userId, 
            includeRelatedEntities: true);
    }

    private User? GetUserFromActiveConnections(int userId)
    {
        return _activeUsers.Keys.FirstOrDefault(u => u.Id == userId);
    }
}

public static class FileHubMethodNames
{
    public const string FileTextForSearch = "TranscribedTextForSearch";
    public const string FileText = "TranscribedText";
}
