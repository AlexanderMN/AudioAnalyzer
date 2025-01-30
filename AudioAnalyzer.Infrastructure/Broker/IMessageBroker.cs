namespace AudioAnalyzer.Infrastructure.Broker;

public interface IMessageBroker
{
    public Task Start();
    public void Stop();
    public Task Subscribe(string topic);
    public Task Publish(string topic, string message);
    public Task<bool> AddConsumer(string topic, Action<object, BrokerEventArgs> callback);
}