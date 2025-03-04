namespace AudioAnalyzer.Data.Persistence.Models;

public class Endpoint
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string IPAddress { get; set; }
    public int Port { get; set; }
    
    public string? Username { get; set; }
    public string? Password { get; set; }
}
