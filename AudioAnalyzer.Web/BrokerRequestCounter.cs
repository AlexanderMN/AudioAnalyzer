using System.Collections.Concurrent;

namespace AudioAnalyzer.Web;

public class BrokerRequestCounter
{
    private ConcurrentDictionary<int, int> _requestCompletedCounts { get; set; }
    public BrokerRequestCounter()
    {
        _requestCompletedCounts = new ConcurrentDictionary<int, int>();
    }
    
    public void AddRequest(int requestId)
    {
        if (RequestAlreadyExists(requestId))
            _requestCompletedCounts[requestId]++;
        else
            CreateRequest(requestId);
    }

    public void RemoveRequest(int requestId)
    {
        _requestCompletedCounts.TryRemove(requestId, out _);
    }

    public bool TryGetCurrentRequestCount(int requestId, out int count)
    {
        return _requestCompletedCounts.TryGetValue(requestId, out count);
    }
    
    private bool RequestAlreadyExists(int requestId)
    {
        return _requestCompletedCounts.ContainsKey(requestId);
    }

    private void CreateRequest(int requestId)
    {
        _requestCompletedCounts[requestId] = 1;
    }
    
    
}
