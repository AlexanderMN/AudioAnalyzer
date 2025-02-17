using System.Runtime.InteropServices.Marshalling;
using System.Text;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace AudioAnalyzer.Infrastructure.Broker;

public class RabbitMqMessagePublisher : IRabbitMqPublisher
{
    private readonly RabbitMqSetting _rabbitMqSetting;

    public RabbitMqMessagePublisher(IOptions<RabbitMqSetting> rabbitMqSetting)
    {
        _rabbitMqSetting = rabbitMqSetting.Value;
    }
    
    public async Task PublishMessageAsync(string message, string topic)
    {
        var factory = new ConnectionFactory
        {
            HostName = _rabbitMqSetting.HostName,
            UserName = _rabbitMqSetting.UserName,
            Password = _rabbitMqSetting.Password,
            Port = _rabbitMqSetting.Port,
        };
        await using var connection = await factory.CreateConnectionAsync();
        await using var channel = await connection.CreateChannelAsync();
        
        await channel.QueueDeclareAsync(queue: topic, durable: false, exclusive: false, autoDelete: false,
                                         arguments: null);
        
        var body = Encoding.UTF8.GetBytes(message);
        if (channel != null)
            await channel.BasicPublishAsync(exchange: String.Empty, routingKey: topic, body: body);
    }
}
