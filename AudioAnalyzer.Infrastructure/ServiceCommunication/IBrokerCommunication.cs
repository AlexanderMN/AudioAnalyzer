using AudioAnalyzer.Infrastructure.Broker;

namespace AudioAnalyzer.Infrastructure.ServiceCommunication;

public interface IBrokerCommunication
{
    public Task ExchangeMessagesAsync(string topicToSendTo,
                                      string messageToSend);
}
