using System.Text.Json.Serialization;

namespace AudioAnalyzer.Web.Models.AudioRequests.SearchRequest;

public class SearchRequest : RequestBase
{
    [JsonPropertyName("requestId")]
    public int RequestId { get; set; }

    [JsonPropertyName("fileOrderId")] 
    public int FileOrderId { get; set; }

    public SearchRequest(int userId,
                             int fileId,
                             int fileOrderId,
                             int requestId,
                             string callbackQueueName) : base(userId, fileId, callbackQueueName)
    {
        FileOrderId = fileOrderId;
        RequestId = requestId;
    }
}
