namespace AudioAnalyzer.Infrastructure.Broker;

public class RabbitMqSetting
{
    public string? HostName { get; set; }
    public string? UserName { get; set; }
    public string? Password { get; set; }
    public int Port { get; set; }
}

public static class BrokerQueues
{
    public const string AudioFileQueue = "Audio-url";
    public const string SearchQueue = "Search";
    public const string TranscribeQueue = "Transcribe";
}
