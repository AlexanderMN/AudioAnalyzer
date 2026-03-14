namespace RabbitMqInfrastructure.Broker;

public class RabbitMqConfig
{
    private IMessageBroker _messageBroker;

    public RabbitMqConfig(IMessageBroker messageBroker)
    {
        _messageBroker = messageBroker;
    }

    public void RegisterConsumers()
    {
        
    }
    
   
}
