using System.Collections.Concurrent;
using System.Text;
using System.Xml.Schema;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace AudioAnalyzer.Infrastructure.Broker;

public class RabbitMqMessageBroker
{
    private IConnection? _connection;
    private IChannel? _channel;
    
    public RabbitMqMessageBroker()
    {
        
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
    }
    
    public async Task Subscribe(string topic, Action<object, BrokerEventArgs> callback)
    {
        if (_channel == null ) 
            return;
           
        await _channel.QueueDeclareAsync(
            queue: topic, 
            durable: true,
            exclusive: false,
            autoDelete: false);
        
        var consumer = new AsyncEventingBasicConsumer(_channel);
        
        consumer.ReceivedAsync += (model,  ea) =>
        {
            BrokerEventArgs eventArgs = new BrokerEventArgs(ea.RoutingKey, ea.Body.ToArray());
            callback(model, eventArgs);

            return Task.CompletedTask;
        };
        
        await _channel.BasicConsumeAsync(
            queue: topic,
            autoAck: true,
            consumer: consumer);

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