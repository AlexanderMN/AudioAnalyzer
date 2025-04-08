using System.ComponentModel.DataAnnotations;


namespace AudioAnalyzer.Data.Persistence.Models;

public class EndPointType
{
    public int Id {get;set;}
    public string Name {get;set;}
    public IEnumerable<Endpoint> Endpoints {get;set;}
}
