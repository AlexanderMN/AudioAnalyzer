using AudioAnalyzer.Data.Models;

namespace AudioAnalyzer.Infrastructure.ServiceCommunication;

public class EndpointService
{
    public static string GetEndpointUri(Endpoint endpoint, EndpointProtocol endpointProtocol, string internalPath = "")
    {
        var uri = endpointProtocol + "://" +
                  endpoint.Username + ":" + 
                  endpoint.Password + "@" +
                  endpoint.IPAddress + ":" + 
                  endpoint.Port + internalPath;
        return uri;
    }
}

public enum EndpointProtocol
{
    http,
    https,
    ftp
}