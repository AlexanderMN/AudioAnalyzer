using System.Text.Json.Serialization;
using AudioAnalyzer.Web.Models.AudioResponses;

namespace AudioAnalyzer.Web.Models.AudioRequests.SummaryRequest;

public class SummaryRequest : RequestBase
{
    [JsonPropertyName("requestId")]
    public int RequestId { get; set; }

    [JsonPropertyName("fileOrderId")] 
    public int FileOrderId { get; set; }

    public SummaryRequest(int userId,
                          int fileId,
                          int fileOrderId,
                          int requestId,
                          string callbackQueueName) : base(userId, fileId, callbackQueueName)
    {
        RequestId = requestId;
        FileOrderId = fileOrderId;
    }
}
