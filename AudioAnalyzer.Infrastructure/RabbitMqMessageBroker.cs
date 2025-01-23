using System.Text;
using RabbitMQ.Client;

namespace AudioAnalyzer.Infrastructure;

public class RabbitMqMessageBroker : IMessageBroker
{
    private IConnection? _connection;
    private IChannel? _channel;
    
    public async void Start(string stringUri)
    {
        var factory = new ConnectionFactory
        {
            Uri = new Uri(stringUri)
        };
        _connection ??= await factory.CreateConnectionAsync();
        _channel ??= await _connection.CreateChannelAsync();
    }

    public void Stop()
    {
        _connection?.Dispose();
        _channel?.Dispose();
    }

    public async void Subscribe(string topic)
    {
        if (_channel != null)
            await _channel.QueueDeclareAsync(queue: topic);
    }

    public async void Publish(string topic, string message)
    {
        var body = Encoding.UTF8.GetBytes(message);
        if (_channel != null)
            await _channel.BasicPublishAsync(exchange: String.Empty, routingKey: topic, body: body);
    }
}