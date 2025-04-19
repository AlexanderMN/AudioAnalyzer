using System.Text;
using System.Text.Json;
using AudioAnalyzer.Data;
using AudioAnalyzer.Data.Persistence.Models;
using AudioAnalyzer.Web.Hubs;
using AudioAnalyzer.Web.Models.AudioResponses;
using AudioAnalyzer.Web.Models.AudioResponses.SplitResponse;
using AudioAnalyzer.Web.Models.AudioResponses.TranscribeResponse;
using Microsoft.AspNetCore.SignalR;
using RabbitMqInfrastructure.Broker;

namespace AudioAnalyzer.Web;

/// <summary>
/// Class contains callbacks for message broker
/// Each callback is added to BrokerQueueCallbacks property: Callbacks via reflection
/// Each private method name should be the same as broker queue name
/// </summary>
public class RabbitMqQueueCallbacks : BrokerQueueCallbacks
{
    private readonly FileUploadHubConnectionContext _connectionContext;
    private DatabaseService _databaseService;
    public RabbitMqQueueCallbacks(FileUploadHubConnectionContext connectionContext,
                                  IConfiguration configuration)
    {
        _connectionContext = connectionContext;
        DataBaseContext dbContext = new DataBaseContext(configuration);
        _databaseService = new DatabaseService(dbContext);
        RegisterDelegates(typeof(RabbitMqQueueCallbacks));
    }
    
    private async Task Search(object state, BrokerEventArgs args)
    {
        string text = Encoding.UTF8.GetString(args.Message);
        var jsonResponse = JsonSerializer.Deserialize<TranscribedResponseJson>(text);
        
        if (jsonResponse == null)
            return;
        await _connectionContext.SendTranscribedTextForSearch(connectionContext: _connectionContext, 
                                                       userId: jsonResponse.AudioResponses[0].UserId, 
                                                       transcribedTextJson: jsonResponse);
    }

    private async Task Transcribe(object state, BrokerEventArgs args)
    {
        string text = Encoding.UTF8.GetString(args.Message, 0, args.Message.Length);
        var jsonResponse = JsonSerializer.Deserialize<TranscribedResponseJson>(text);

        if (jsonResponse?.AudioResponses[0].Response is TranscribedText transcribedText)
        {
            await _connectionContext.SendTranscribedText(connectionContext: _connectionContext, 
                                                  userId: jsonResponse.AudioResponses[0].UserId, 
                                                  text: transcribedText.Text);   
        }
    }

    private async Task Summarize(object state, BrokerEventArgs args)
    {
        string text = Encoding.UTF8.GetString(args.Message, 0, args.Message.Length);
    }

    private async Task Spectrogram(object state, BrokerEventArgs args)
    {
        string text = Encoding.UTF8.GetString(args.Message, 0, args.Message.Length);
    }

    private async Task SplitResult(object state, BrokerEventArgs args)
    {
        string text = Encoding.UTF8.GetString(args.Message, 0, args.Message.Length);

        var splitResponse = JsonSerializer.Deserialize<SplitResponse>(text);
        
        if (splitResponse == null)
            return;
        
        var uploadedFile = await _databaseService.UploadedFileRepository
                                                 .GetEntity(splitResponse.FileId, false);
        if (uploadedFile == null)
            return;
        
        if (splitResponse.ResponseCode == 1)
        {
            uploadedFile.FileState = FileState.Error;
            _databaseService.UploadedFileRepository.Update(uploadedFile);
            await _databaseService.UploadedFileRepository.SaveAsync();
            return;
        }
        
        uploadedFile.FileState = FileState.Ready;
        uploadedFile.Duration = splitResponse.Duration;
        _databaseService.UploadedFileRepository.Update(uploadedFile);
        await _databaseService.UploadedFileRepository.SaveAsync();
    }
}
