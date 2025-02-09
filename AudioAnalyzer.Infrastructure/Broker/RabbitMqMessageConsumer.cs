using System.Collections.Concurrent;
using System.Text;
using System.Xml.Schema;
using AudioAnalyzer.Infrastructure.ServiceCommunication;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace AudioAnalyzer.Infrastructure.Broker;

public class RabbitMqMessageConsumer: IHostedService
{
    private IConnection? _connection;
    private IChannel? _channel;

    private readonly RabbitMqSetting _rabbitMqSetting;
    protected readonly BrokerQueueCallbacks BrokerQueueCallbacks;
    public RabbitMqMessageConsumer(IOptions<RabbitMqSetting> rabbitMqSetting,
                                   BrokerQueueCallbacks brokerQueueCallbacks)
    {
        _rabbitMqSetting = rabbitMqSetting.Value;
        BrokerQueueCallbacks = brokerQueueCallbacks;
    }

    private async Task Start()
    {
        var factory = new ConnectionFactory
        {
            HostName = _rabbitMqSetting.HostName,
            Port = _rabbitMqSetting.Port,
            UserName = _rabbitMqSetting.UserName,
            Password = _rabbitMqSetting.Password,
        };
        _connection ??= await factory.CreateConnectionAsync();
        _channel ??= await _connection.CreateChannelAsync();
    }

    public void Dispose()
    {
        _channel?.Dispose();
        _connection?.Dispose();
    }

    public async Task Subscribe(string topic, Func<object, BrokerEventArgs, Task> callback)
    {
        if (_channel == null ) 
            return;
           
        await _channel.QueueDeclareAsync(
            queue: topic, 
            durable: true,
            exclusive: false,
            autoDelete: false);
        
        var consumer = new AsyncEventingBasicConsumer(_channel);
        
        consumer.ReceivedAsync += async (model,  ea) =>
        {
            BrokerEventArgs eventArgs = new BrokerEventArgs(ea.RoutingKey, ea.Body.ToArray());
            await callback(model, eventArgs);
        };
        
        await _channel.BasicConsumeAsync(
            queue: topic,
            autoAck: true,
            consumer: consumer);
    }

    protected async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        
        foreach (var eventDelegate in BrokerQueueCallbacks.Callbacks)
        {
            await Subscribe(eventDelegate.Key, (Func<object, BrokerEventArgs, Task>)eventDelegate.Value);
        }
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await this.Start();
        await ExecuteAsync(cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        Dispose();
        
        return Task.CompletedTask;
    }
}