namespace RabbitMqInfrastructure.Broker;

public interface IMessageBroker
{
    public Task Start();
    public void Stop();
    public Task Subscribe(string topic, Action<object, BrokerEventArgs> callback);
    
    public Task Publish(string topic, string message);
}