namespace AudioAnalyzer.Infrastructure.Broker;

public abstract class BrokerQueueCallbacks
{
    public Dictionary<string, Delegate> Callbacks { get; set;}

    protected BrokerQueueCallbacks()
    {
        Callbacks = new Dictionary<string, Delegate>();
    }
}
