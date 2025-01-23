using Microsoft.Extensions.Primitives;

namespace AudioAnalyzer.Web.Services.EndpointService;

public interface IEndpointService <TKey>
{
    public string GetUriFromEndpointId(TKey endPointId, string internalPath = "");
    public Task<HttpResponseMessage> PostFileToEndpointAsync(string uri, Stream dataStream, Dictionary<string, string> requestHeaders, Dictionary<string, StringValues> fileHeaders);
    public Task<HttpResponse> GetDataFromEndpoint(string uri, Stream dataStream, HeaderDictionary headers);
}