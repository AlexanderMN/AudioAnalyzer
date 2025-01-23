using Microsoft.Extensions.Primitives;

namespace AudioAnalyzer.Web.Services.EndpointService;

public interface IEndpointService <TKey>
{
    public string GetUriFromEndpointId(TKey endPointId, EndpointProtocol endpointProtocol, string internalPath = "");
    public Task<HttpResponse> GetDataFromEndpoint(string uri, Stream dataStream, HeaderDictionary headers);
}

public enum EndpointProtocol
{
    http,
    https,
    ftp
}