using AudioAnalyzer.Data.Persistence.Models;
using AudioAnalyzer.Data.Persistence.Repositories;

namespace AudioAnalyzer.Infrastructure.ServiceCommunication;

public class EndpointService
{
    private IRepository<Endpoint> _endpointRepo;
    private HttpClient _httpClient;

    public EndpointService(IRepository<Endpoint> endpointRepo, HttpClient httpClient)
    {
        _endpointRepo = endpointRepo;
        _httpClient = httpClient;
    }

    public Endpoint? GetAvailableEndpoint(int endPointTypeId)
    {
        var endpoints = _endpointRepo.GetEntityList(
            (e => e.EndPointTypeId == endPointTypeId)
            );
        
        return endpoints.FirstOrDefault();
    }

    public async Task<Endpoint?> GetEndpoint(int endPointId)
    {
        var endpoint = await _endpointRepo.GetEntity(
            id: endPointId,
            includeRelatedEntities: false);
        
        return endpoint;
    }

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