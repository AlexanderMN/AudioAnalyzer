namespace AudioAnalyzer.Infrastructure.Broker;

public interface IRabbitMqPublisher
{
    public Task PublishMessageAsync(string message, string topic);
}
