using System.Text;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace RabbitMqInfrastructure.Broker;

public class RabbitMqMessagePublisher : IRabbitMqPublisher
{
    private readonly RabbitMqSetting _rabbitMqSetting;

    public RabbitMqMessagePublisher(RabbitMqSetting rabbitMqSetting)
    {
        _rabbitMqSetting = rabbitMqSetting;
    }
    
    public async Task PublishMessageAsync(string message, string queue)
    {
        var factory = new ConnectionFactory
        {
            HostName = _rabbitMqSetting.IpAddress,
            UserName = _rabbitMqSetting.UserName,
            Password = _rabbitMqSetting.Password,
            Port = _rabbitMqSetting.Port
        };
        await using var connection = await factory.CreateConnectionAsync();
        await using var channel = await connection.CreateChannelAsync();
        
        await channel.QueueDeclareAsync(queue: queue, durable: true, exclusive: false, autoDelete: false,
                                         arguments: null);
        
        var body = Encoding.UTF8.GetBytes(message);
        if (channel != null)
            await channel.BasicPublishAsync(exchange: String.Empty, routingKey: queue, body: body);
    }
}
