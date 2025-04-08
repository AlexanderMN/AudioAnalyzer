using System.Text;
using System.Text.Json;
using AudioAnalyzer.Web.Hubs;
using AudioAnalyzer.Web.Models.AudioResponse;
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
    public RabbitMqQueueCallbacks(IHubContext<FileUploadHub> hubContext,
                                  FileUploadHubConnectionContext connectionContext)
    {
        _connectionContext = connectionContext;
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
        
        if (jsonResponse == null)
            return;
        if (jsonResponse.AudioResponses[0].Response is TranscribedText transcribedText)
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

        var audioResponse = JsonSerializer.Deserialize<AudioResponse>(text);
        
    }
}
