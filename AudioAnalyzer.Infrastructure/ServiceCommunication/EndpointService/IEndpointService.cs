namespace AudioAnalyzer.Infrastructure.ServiceCommunication.EndpointService;

public interface IEndpointService <TKey>
{
    public string GetUriFromEndpointId(TKey endPointId, EndpointProtocol endpointProtocol, string internalPath = "");
}

public enum EndpointProtocol
{
    http,
    https,
    ftp
}