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
                                            string messageToSend,
                                            string topicToAwaitFrom,
                                            Action<object, BrokerEventArgs> onReceive)
    {
        TaskCompletionSource completion = new TaskCompletionSource();
        await _messageBroker.Subscribe(topicToAwaitFrom, onReceive, completion);
        await _messageBroker.Publish(topicToSendTo, messageToSend);
        await completion.Task;
    }
}