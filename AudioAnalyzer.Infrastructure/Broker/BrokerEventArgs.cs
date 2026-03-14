namespace RabbitMqInfrastructure.Broker;

public class BrokerEventArgs: EventArgs
{
    public string Topic { get; set; }
    public byte[] Message { get; set; }

    public BrokerEventArgs(string topic, byte[] message)
    {
        Topic = topic;
        Message = message;
    }
}