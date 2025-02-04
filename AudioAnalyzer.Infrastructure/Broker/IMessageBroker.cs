namespace AudioAnalyzer.Infrastructure.Broker;

public interface IMessageBroker
{
    public Task Start();
    public void Stop();
    public Task Subscribe(string topic, Action<object, BrokerEventArgs> callback, TaskCompletionSource completion);
    
    public Task Publish(string topic, string message);
}