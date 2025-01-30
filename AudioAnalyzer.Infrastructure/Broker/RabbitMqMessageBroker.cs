using System.Collections.Concurrent;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace AudioAnalyzer.Infrastructure.Broker;

public class RabbitMqMessageBroker : IMessageBroker
{
    private IConnection? _connection;
    private IChannel? _channel;
    public List<string> Topics;
    public  ConcurrentDictionary<string, AsyncEventingBasicConsumer> Consumers;
    
    public RabbitMqMessageBroker()
    {
        Consumers = new ConcurrentDictionary<string, AsyncEventingBasicConsumer>();
        Start();
    }
    
    public async Task Start()
    {
        var factory = new ConnectionFactory
        {
            HostName = "127.0.0.1",
            Port = 5672,
            UserName = "guest",
            Password = "guest",
        };
        _connection ??= await factory.CreateConnectionAsync();
        _channel ??= await _connection.CreateChannelAsync();
        
        
        return;
    }

    public void Stop()
    {
        _connection?.Dispose();
        _channel?.Dispose();
        
    }


    public async Task<bool> AddConsumer(string topic, Action<object, BrokerEventArgs> callback)
    {
        if (_channel == null) 
            return false;
        
        await _channel.QueueDeclareAsync(queue: topic);
        var consumer = new AsyncEventingBasicConsumer(_channel);

        consumer.ReceivedAsync += (model, ea) =>
        {
            BrokerEventArgs eventArgs = new BrokerEventArgs(ea.RoutingKey, ea.Body.ToArray());
            callback(model, eventArgs);
            return Task.CompletedTask;
        };
        
        return Consumers.TryAdd(topic, consumer);
    }
    
    public async Task Subscribe(string topic)
    {
        
        await _channel.BasicConsumeAsync(
            queue: topic,
            autoAck: true,
            consumer: Consumers[topic]);

        return;
    }

    public async Task Publish(string topic, string message)
    {
        if (_channel == null) 
            return;
        
        await _channel.QueueDeclareAsync(queue: topic, durable: false, exclusive: false, autoDelete: false,
            arguments: null);
            
        var body = Encoding.UTF8.GetBytes(message);
        if (_channel != null)
            await _channel.BasicPublishAsync(exchange: String.Empty, routingKey: topic, body: body);
        
    }
}