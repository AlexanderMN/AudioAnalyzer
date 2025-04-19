using System.Collections.Concurrent;
using System.Text.Json;
using AudioAnalyzer.Data.Persistence.Models;
using AudioAnalyzer.Web.Models.AudioResponses;
using AudioAnalyzer.Web.Models.AudioResponses.TranscribeResponse;
using Microsoft.AspNetCore.SignalR;

namespace AudioAnalyzer.Web.Hubs;

public class FileUploadHubConnectionContext
{
    internal readonly ConcurrentDictionary<User, byte> ActiveUsers;
    private readonly IHubContext<FileUploadHub> _hubContext;
    
    public FileUploadHubConnectionContext(IHubContext<FileUploadHub> hubContext)
    {
        _hubContext = hubContext;
        ActiveUsers = new ConcurrentDictionary<User, byte>();
    }
    
    public async Task SendTranscribedText(FileUploadHubConnectionContext connectionContext,
                                          int userId, 
                                          string text)
    {
        var user = connectionContext.ActiveUsers.Keys.FirstOrDefault(u => u.Id == userId);
        
        if (user == null)
            return;
        
        await _hubContext.Clients.Client(user.ConnectionId).SendAsync(FileHubMethodNames.FileText, text);
    }


    public async Task SendTranscribedTextForSearch(FileUploadHubConnectionContext connectionContext,
                                                          int userId, 
                                                          TranscribedResponseJson transcribedTextJson)
    {
        var user = connectionContext.ActiveUsers.Keys.FirstOrDefault(u => u.Id == userId);
        
        if (user == null)
            return;
        
        var text = JsonSerializer.Serialize(transcribedTextJson);

        await _hubContext.Clients.Client(user.ConnectionId).SendAsync(FileHubMethodNames.FileTextForSearch, text);
    }
}
