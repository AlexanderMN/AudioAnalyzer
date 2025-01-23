namespace AudioAnalyzer.Infrastructure;

public interface IMessageBroker
{
    public void Start(string stringUri);
    public void Stop();
    public void Subscribe(string topic);
    public void Publish(string topic, string message);
}