using System.Collections.Concurrent;
using System.Text;
using System.Text.Json;
using AudioAnalyzer.Data;
using AudioAnalyzer.Data.Models;
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
    private readonly DatabaseDbContextService _databaseDbContextService;
    
    private ConcurrentDictionary<int, int> _requestCompletedCounts { get; set; }
    public RabbitMqQueueCallbacks(FileUploadHubConnectionContext connectionContext,
                                  IConfiguration configuration)
    {
        _connectionContext = connectionContext;
        DataBaseContext dbContext = new DataBaseContext(configuration);
        _databaseDbContextService = new DatabaseDbContextService(dbContext);
        _requestCompletedCounts = new ConcurrentDictionary<int, int>();
        
        RegisterDelegates(typeof(RabbitMqQueueCallbacks));
    }
    
    private async Task Search(object state, BrokerEventArgs args)
    {
        string text = Encoding.UTF8.GetString(args.Message);
        var jsonResponse = JsonSerializer.Deserialize<TranscribeResponse>(text);
        
        if (jsonResponse == null)
            return;
        await _connectionContext.SendTranscribedTextForSearch(connectionContext: _connectionContext, 
                                                       userId: jsonResponse.UserId, 
                                                       transcribedTextJson: jsonResponse);
    }

    private async Task Transcribe(object state, BrokerEventArgs args)
    {
        string text = Encoding.UTF8.GetString(args.Message, 0, args.Message.Length);
        var jsonResponse = JsonSerializer.Deserialize<TranscribeResponse>(text);

        
        if (jsonResponse == null)
            return;
        
        if (jsonResponse.ResponseCode == 1)
        {
            return;
        }
        
        var fileRequestedEvent = await _databaseDbContextService
            .GetFileRequestedEventByIndex(
                fileId: jsonResponse.FileId,
                requestId: jsonResponse.RequestId);
        
        //TODO add error
        if (fileRequestedEvent == null)
            return;
        
        var audioResponse = new AudioResponse
        {
            OrderId = jsonResponse.FileOrderId,
            ResponseText = jsonResponse.Response.ToString()!,
            ResponseType = AudioResponseType.Success,
            FileRequestedEvent = fileRequestedEvent
        };
        
        _databaseDbContextService.AudioResponseRepository.Create(audioResponse);
        await _databaseDbContextService.AudioResponseRepository.SaveAsync();
        
        _requestCompletedCounts[jsonResponse.UserId]++;

            
        if (_requestCompletedCounts[jsonResponse.UserId] == fileRequestedEvent.UploadedFile.SplitNumber)
        {
            await _connectionContext.SendTranscribedText(connectionContext: _connectionContext, 
                                                  userId: jsonResponse.UserId, 
                                                  text: jsonResponse.Response.ToString()!);   
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
        
        var uploadedFile = await _databaseDbContextService.UploadedFileRepository
                                                 .GetEntity(splitResponse.FileId, false);
        if (uploadedFile == null)
            return;
        
        if (splitResponse.ResponseCode == 1)
        {
            uploadedFile.FileState = FileState.Error;
            _databaseDbContextService.UploadedFileRepository.Update(uploadedFile);
            await _databaseDbContextService.UploadedFileRepository.SaveAsync();
            return;
        }
        
        uploadedFile.FileState = FileState.Ready;
        uploadedFile.Duration = splitResponse.Duration;
        uploadedFile.SplitNumber = splitResponse.SplitNumber;
        
        _databaseDbContextService.UploadedFileRepository.Update(uploadedFile);
        await _databaseDbContextService.UploadedFileRepository.SaveAsync();
    }
}
