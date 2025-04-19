using System.Text.Json;
using AudioAnalyzer.Data.Persistence.Models;
using AudioAnalyzer.Web.Models.AudioRequests.SplitRequest;
using AudioAnalyzer.Web.Models.AudioRequests.TranscribeRequest;
using RabbitMqInfrastructure.Broker;

namespace AudioAnalyzer.Web;

public class RabbitMqPostManager
{
    private IRabbitMqPublisher _rabbitMqPublisher;

    public RabbitMqPostManager(IRabbitMqPublisher rabbitMqPublisher)
    {
        _rabbitMqPublisher = rabbitMqPublisher;
    }


    public async Task PostSplitRequest(
        User user,
        int fileId,
        string queueName,
        string callbackQueueName)
    {
        var splitRequest = new SplitRequest
        {
            UserId = user.Id,
            FileId = fileId,
            CallbackQueue = callbackQueueName
        };

        var message = JsonSerializer.Serialize(splitRequest);

        await _rabbitMqPublisher.PublishMessageAsync(message: message,
                                                     queue: BrokerQueues.SplitQueue);

    }

    public async Task PostTranscribeRequest(AudioRequest audioRequest,
                                            int fileId,
                                            string callbackQueueName)
    {

        var transcribeRequest = new TranscribeRequest
        {
            UserId = audioRequest.UserId,
            FileId = fileId,
            RequestId = audioRequest.Id,
            CallbackQueue = callbackQueueName
        };

        var message = JsonSerializer.Serialize(transcribeRequest);
        await _rabbitMqPublisher.PublishMessageAsync(message, BrokerQueues.AudioFileQueue);

    }

    public async Task PostSearchRequest(AudioRequest audioRequest,
                                        int fileId,
                                        string callbackQueueName)
    {
        
    }
}
