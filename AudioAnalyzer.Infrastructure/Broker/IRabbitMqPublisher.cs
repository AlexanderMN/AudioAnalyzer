namespace RabbitMqInfrastructure.Broker;

public interface IRabbitMqPublisher
{
    public Task PublishMessageAsync(string message, string queue);
}
