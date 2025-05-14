using System.Text.Json;
using AudioAnalyzer.Data.Models;
using AudioAnalyzer.Web.Models.AudioRequests;
using AudioAnalyzer.Web.Models.AudioRequests.PreprocessRequest;
using AudioAnalyzer.Web.Models.AudioRequests.SearchRequest;
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
    
    
    public async Task PostSystemRequestToService(
        User user,
        int fileId,
        string queueName,
        string callbackQueueName)
    {
        var splitRequest = new PreprocessRequest(user.Id, fileId, callbackQueueName);
    
        var message = JsonSerializer.Serialize(splitRequest);
    
        await _rabbitMqPublisher.PublishMessageAsync(message: message,
                                                     queue: queueName);
    }

    public async Task PostUserRequestToService<T>(AudioRequest audioRequest,
                                            int fileId,
                                            string callbackQueueName, 
                                            int fileOrderId = 0) where T : RequestBase
    {
        RequestBase request;
        
        if (typeof(T) == typeof(TranscribeRequest))
            request = new TranscribeRequest(audioRequest.UserId, fileId, fileOrderId, audioRequest.Id, callbackQueueName);
        else
        {
            request = new SearchRequest(audioRequest.UserId, fileId, fileOrderId, audioRequest.Id, callbackQueueName);
        }
        
        var message = JsonSerializer.Serialize((T)request);
        await _rabbitMqPublisher.PublishMessageAsync(message, BrokerQueues.AudioFileQueue);
    }
}
