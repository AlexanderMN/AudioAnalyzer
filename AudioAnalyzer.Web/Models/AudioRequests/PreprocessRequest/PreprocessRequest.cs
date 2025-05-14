namespace AudioAnalyzer.Web.Models.AudioRequests.PreprocessRequest;

public class PreprocessRequest : RequestBase
{
    public PreprocessRequest(int userId, int fileId, string callbackQueueName) : base(userId, fileId, callbackQueueName)
    {
        
    }
}
