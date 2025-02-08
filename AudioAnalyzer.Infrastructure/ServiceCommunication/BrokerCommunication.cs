using AudioAnalyzer.Infrastructure.Broker;

namespace AudioAnalyzer.Infrastructure.ServiceCommunication;

public class BrokerCommunication : IBrokerCommunication
{
    private IMessageBroker _messageBroker;

    public BrokerCommunication(IMessageBroker messageBroker)
    {
        _messageBroker = messageBroker;
    }

    public async Task ExchangeMessagesAsync(string topicToSendTo,
                                            string messageToSend)
    {
        await _messageBroker.Publish(topicToSendTo, messageToSend);
        
    }
}