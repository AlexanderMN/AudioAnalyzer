using System.Text;
using System.Text.Json;

namespace AudioAnalyzer.Infrastructure.Broker;

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
