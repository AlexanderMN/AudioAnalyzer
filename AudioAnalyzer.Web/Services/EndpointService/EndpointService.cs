using AudioAnalyzer.Web.Models.Persistence.Repositories.Endpoints;
using Microsoft.Extensions.Primitives;

namespace AudioAnalyzer.Web.Services.EndpointService;

public class EndpointService<TKey> : IEndpointService<TKey> where TKey : IComparable<TKey>, IEquatable<TKey>
{
    private IEndpointRepository<TKey> _endpointRepository;
    private HttpClient _httpClient;
    
    public EndpointService(IEndpointRepository<TKey> endpointRepository, HttpClient httpClient)
    {
        _endpointRepository = endpointRepository;
        _httpClient = httpClient;
    }

    public string GetUriFromEndpointId(TKey endPointId, string internalPath = "")
    {
        var endpoint = _endpointRepository.GetEndpoint(endPointId);
            
        var uri = "http://" + 
                  endpoint.Address.ToString() + ":" + 
                  endpoint.Port + internalPath;
        return uri;
    }

    public Task<HttpResponseMessage> PostFileToEndpointAsync(string uri, Stream dataStream, 
        Dictionary<string, string> requestHeaders, Dictionary<string, StringValues> fileHeaders)
    {
        var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, uri);
        
        foreach (var header in requestHeaders)
        {
            httpRequestMessage.Headers.Add(header.Key, header.Value.ToString());
        }
        
        var streamContent = new StreamContent(dataStream);
        
        
        MultipartFormDataContent formDataContent = new MultipartFormDataContent();

        foreach (var fileHeader in fileHeaders)
        {
            streamContent.Headers.Add(fileHeader.Key, fileHeader.Value.ToString());
        }
        //streamContent.Headers.Add("Content-Length", dataStream.Length.ToString());
        
        formDataContent.Add(streamContent);
        
        httpRequestMessage.Content = formDataContent;
        
        //TODO fix this
        //httpRequestMessage.Content.

        _httpClient.Timeout = new TimeSpan(0, 5, 0);
        return _httpClient.SendAsync(httpRequestMessage);
    }

    public Task<HttpResponse> GetDataFromEndpoint(string uri, Stream dataStream, HeaderDictionary headers)
    {
        throw new NotImplementedException();
    }
}