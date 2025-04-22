using System.Text.Json.Serialization;

namespace AudioAnalyzer.Web.Models.AudioRequests.SplitRequest;

public class SplitRequest : RequestBase
{
    public SplitRequest(int userId, int fileId, string callbackQueueName) : base(userId, fileId, callbackQueueName)
    {
        
    }
}
