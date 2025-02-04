using System.Text.Json.Serialization;

namespace AudioAnalyzer.Data.Persistence.Repositories.Endpoints;

public class Endpoints
{
    [JsonPropertyName("RemoteEndpoints")]
    private List<RemoteEndpoint> remoteEndpoints { get; set; }
}
