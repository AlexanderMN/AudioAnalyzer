using System.ComponentModel.DataAnnotations;

namespace AudioAnalyzer.Data.Models;

public class Endpoint
{
    [Key]
    public int Id { get; set; }
    public string Name { get; set; }
    public string IPAddress { get; set; }
    public int Port { get; set; }
    public bool Active { get; set; }
    public string? Username { get; set; }
    public string? Password { get; set; }
    public long? SpaceLeft { get; set; }
    public long? SpaceLimit { get; set; }
    public EndPointType EndPointType { get; set; }
    public IEnumerable<UploadedFile>? UploadedFiles { get; set; }
}

public enum EndPointType
{
    AudioRecognizer,
    Broker,     
    FTPServer,
    HTTPServer, 
    FileSplitter, 
    AudioSummarizer,
    AudioClassifier
}

