namespace RabbitMqInfrastructure.Broker;

public class RabbitMqSetting
{
    public string? IpAddress { get; set; }
    public string? UserName { get; set; }
    public string? Password { get; set; }
    public int Port { get; set; }
}

public static class BrokerQueues
{
    public const string AudioFileQueue = "Audio-url";
    public const string SearchQueue = "Search";
    public const string TranscribeQueue = "Transcribe";
    public const string SummaryQueue = "Summary";
    public const string ClassificationQueue = "Classification";
    public const string PreprocessQueue = "Preprocess";
    public const string PreprocessResultQueue = "PreprocessResult";
}
