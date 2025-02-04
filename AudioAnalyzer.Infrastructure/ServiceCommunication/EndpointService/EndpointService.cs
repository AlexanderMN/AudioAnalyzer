using AudioAnalyzer.Data.Persistence.Repositories.Endpoints;

namespace AudioAnalyzer.Infrastructure.ServiceCommunication.EndpointService;

public class EndpointService<TKey> : IEndpointService<TKey> where TKey : IComparable<TKey>, IEquatable<TKey>
{
    private IEndpointRepository<TKey> _endpointRepository;
    private HttpClient _httpClient;
    
    public EndpointService(IEndpointRepository<TKey> endpointRepository, HttpClient httpClient)
    {
        _endpointRepository = endpointRepository;
        _httpClient = httpClient;
    }

    public string GetUriFromEndpointId(TKey endPointId, EndpointProtocol endpointProtocol, string internalPath = "")
    {
        var endpoint = _endpointRepository.GetEndpoint(endPointId);
            
        var uri = endpointProtocol + "://" + 
                  endpoint.Address + ":" + 
                  endpoint.Port + internalPath;
        return uri;
    }
    
}