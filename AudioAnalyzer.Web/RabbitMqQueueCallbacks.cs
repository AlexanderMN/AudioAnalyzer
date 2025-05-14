using System.Collections.Concurrent;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using AudioAnalyzer.Data;
using AudioAnalyzer.Data.Models;
using AudioAnalyzer.Web.Hubs;
using AudioAnalyzer.Web.Models.AudioResponses;
using AudioAnalyzer.Web.Models.AudioResponses.PreprocessResponse;
using AudioAnalyzer.Web.Models.AudioResponses.SearchResponse;
using AudioAnalyzer.Web.Models.AudioResponses.SummaryResponse;
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
    private readonly BrokerRequestCounter _brokerRequestCounter;
    public RabbitMqQueueCallbacks(FileUploadHubConnectionContext connectionContext,
                                  IConfiguration configuration)
    {
        _connectionContext = connectionContext;
        DataBaseContext dbContext = new DataBaseContext(configuration);
        _databaseDbContextService = new DatabaseDbContextService(dbContext);
        _brokerRequestCounter = new BrokerRequestCounter();
        RegisterDelegates(typeof(RabbitMqQueueCallbacks));
    }
    
    private async Task Search(object state, BrokerEventArgs args)
    {
        string text = Encoding.UTF8.GetString(args.Message);
        var searchResponse = JsonSerializer.Deserialize<SearchResponse>(text);
        if (searchResponse == null || searchResponse.ResponseCode == 1)
            return;
        
        var fileRequestedEvent = await _databaseDbContextService
            .GetFileRequestedEventByIndex(
                fileId: searchResponse.FileId,
                requestId: searchResponse.RequestId);
        
        //TODO add error
        if (fileRequestedEvent == null)
            return;

        JsonSerializerOptions options = new JsonSerializerOptions
        {
            Encoder = JavaScriptEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.Cyrillic, UnicodeRanges.Arabic)
        };
        
        var textForSearch = JsonSerializer.Serialize(searchResponse.SearchText, options);
        
        var audioResponse = new AudioResponse
        {
            OrderId = searchResponse.FileOrderId,
            ResponseText = textForSearch,
            ResponseType = AudioResponseType.Success,
            FileRequestedEvent = fileRequestedEvent
        };
        
        _databaseDbContextService.AudioResponseRepository.Create(audioResponse);
        await _databaseDbContextService.AudioResponseRepository.SaveAsync();
        
        _brokerRequestCounter.AddRequest(searchResponse.RequestId);
        _brokerRequestCounter.TryGetCurrentRequestCount(searchResponse.RequestId, out var count);
        if (count == -1)
            return;
        if (count == fileRequestedEvent.UploadedFile.SplitNumber)
        {
            await _databaseDbContextService.SetFileRequestedEventState(fileRequestedEvent);
            _brokerRequestCounter.RemoveRequest(searchResponse.RequestId);
        }
    }

    private async Task Transcribe(object state, BrokerEventArgs args)
    {
        string text = Encoding.UTF8.GetString(args.Message, 0, args.Message.Length);
        var transcribeResponse = JsonSerializer.Deserialize<TranscribeResponse>(text);
        if (transcribeResponse == null || transcribeResponse.ResponseCode == 1)
            return;
        
        var fileRequestedEvent = await _databaseDbContextService
            .GetFileRequestedEventByIndex(
                fileId: transcribeResponse.FileId,
                requestId: transcribeResponse.RequestId);

        if (fileRequestedEvent == null)
            return;
        
        var audioResponse = new AudioResponse
        {
            OrderId = transcribeResponse.FileOrderId,
            ResponseText = transcribeResponse.Text,
            ResponseType = AudioResponseType.Success,
            FileRequestedEventId = fileRequestedEvent.Id
        };
        
        _databaseDbContextService.AudioResponseRepository.Create(audioResponse);
        await _databaseDbContextService.AudioResponseRepository.SaveAsync();
        
        _brokerRequestCounter.AddRequest(transcribeResponse.RequestId);
        _brokerRequestCounter.TryGetCurrentRequestCount(transcribeResponse.RequestId, out var count);
        if (count == -1)
            return;
            
        if (count == fileRequestedEvent.UploadedFile.SplitNumber)
        {
            await _databaseDbContextService.SetFileRequestedEventState(fileRequestedEvent);
            _brokerRequestCounter.RemoveRequest(transcribeResponse.RequestId);

        }
    }

    private async Task Summarize(object state, BrokerEventArgs args)
    {
        string text = Encoding.UTF8.GetString(args.Message, 0, args.Message.Length);
        var summaryResponse = JsonSerializer.Deserialize<SummaryResponse>(text);
        if (summaryResponse == null)
            return;
        
        var fileRequestedEvent = await _databaseDbContextService
            .GetFileRequestedEventByIndex(
                fileId: summaryResponse.FileId,
                requestId: summaryResponse.RequestId);
        
        //TODO add error
        if (fileRequestedEvent == null)
            return;
        
        var audioResponse = new AudioResponse
        {
            OrderId = summaryResponse.FileOrderId,
            ResponseText = summaryResponse.Text,
            FileRequestedEventId = fileRequestedEvent.Id,
            ResponseType = summaryResponse.ResponseCode == 0 ? 
                AudioResponseType.Success : 
                AudioResponseType.Error
        };

        _databaseDbContextService.AudioResponseRepository.Create(audioResponse);
        await _databaseDbContextService.AudioResponseRepository.SaveAsync();
        
        _brokerRequestCounter.AddRequest(summaryResponse.RequestId);
        _brokerRequestCounter.TryGetCurrentRequestCount(summaryResponse.RequestId, out var count);
        
        if (count == -1)
            return;
        
        if (count == fileRequestedEvent.UploadedFile.SplitNumber)
        {
           await _databaseDbContextService.SetFileRequestedEventState(fileRequestedEvent);
           _brokerRequestCounter.RemoveRequest(summaryResponse.RequestId);
        }
    }
    
    private async Task PreprocessResult(object state, BrokerEventArgs args)
    {
        string text = Encoding.UTF8.GetString(args.Message, 0, args.Message.Length);
        var preprocessResponse = JsonSerializer.Deserialize<PreprocessResponse>(text);
        if (preprocessResponse == null)
            return;
        
        var uploadedFile = await _databaseDbContextService.UploadedFileRepository
                                                 .GetEntity(preprocessResponse.FileId, false);
        if (uploadedFile == null)
            return;
        
        if (preprocessResponse.ResponseCode == 1)
        {
            uploadedFile.FileState = FileState.Error;
            _databaseDbContextService.UploadedFileRepository.Update(uploadedFile);
            await _databaseDbContextService.UploadedFileRepository.SaveAsync();
            return;
        }
        
        uploadedFile.FileState = FileState.Ready;
        uploadedFile.Duration = preprocessResponse.Duration;
        uploadedFile.SplitNumber = preprocessResponse.SplitNumber;
        
        _databaseDbContextService.UploadedFileRepository.Update(uploadedFile);
        await _databaseDbContextService.UploadedFileRepository.SaveAsync();
        
        
    }
}
