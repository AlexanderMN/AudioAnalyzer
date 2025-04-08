using System.ComponentModel.DataAnnotations;

namespace AudioAnalyzer.Data.Persistence.Models;

public class Endpoint
{
    [Key]
    public int Id { get; set; }
    public string Name { get; set; }
    public string IPAddress { get; set; }
    public int Port { get; set; }
    
    public string? Username { get; set; }
    public string? Password { get; set; }
    
    public int EndPointTypeId { get; set; }
    public EndPointType EndPointType { get; set; }
}
