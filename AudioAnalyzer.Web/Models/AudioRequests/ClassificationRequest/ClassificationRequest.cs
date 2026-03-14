using System.Text.Json.Serialization;

namespace AudioAnalyzer.Web.Models.AudioRequests.ClassificationRequest;

public class ClassificationRequest : RequestBase
{
    [JsonPropertyName("requestId")]
    public int RequestId { get; set; }

    [JsonPropertyName("fileOrderId")] 
    public int FileOrderId { get; set; }

    public ClassificationRequest(int userId,
                                 int fileId,
                                 int fileOrderId,
                                 int requestId,
                                 string callbackQueueName) : base(userId, fileId, callbackQueueName)
    {
        FileOrderId = fileOrderId;
        RequestId = requestId;
    }
}
