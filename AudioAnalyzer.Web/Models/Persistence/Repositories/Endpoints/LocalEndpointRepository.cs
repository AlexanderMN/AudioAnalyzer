using System.Net;

namespace AudioAnalyzer.Web.Models.Persistence.Repositories.Endpoints;

public class LocalEndpointRepository<TKey> : IEndpointRepository<TKey> where TKey : IComparable<TKey>, IEquatable<TKey>
{
    private IConfiguration _configuration;

    public LocalEndpointRepository(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public IDictionary<TKey, IPEndPoint> GetEndpoints() 
    {
        var endpoints = new Dictionary<TKey, IPEndPoint>();
        
        var remoteEndpointSections = GetRemoteEndpointsSection().GetChildren();
        
        foreach (var remoteEndpointSection in remoteEndpointSections)
        {
            
            if (remoteEndpointSection.Value is TKey endpointId && 
                TryExtractEndpointFromSction(remoteEndpointSection, out IPEndPoint endpoint))
            {
                endpoints.Add(endpointId, endpoint);
            }
            
        }
        
        return endpoints;
    }

    public IPEndPoint GetEndpoint(TKey id)
    {
        IPEndPoint endpoint = null;
        var sectionId = id as string;
        
        if (sectionId != null)
        {
            var endpointSection = GetRemoteEndpointsSection().GetSection(sectionId);
        
            TryExtractEndpointFromSction(endpointSection, out endpoint);    
        }
        
        
        return endpoint;
    }

    private IConfigurationSection GetRemoteEndpointsSection()
    {
        return _configuration.GetSection("RemoteEndpoints");
    }

    private bool TryExtractEndpointFromSction(IConfigurationSection section, out IPEndPoint endpoint)
    {
        endpoint = null;
        var ipAddress = section.GetValue<string>("IPAddress");
        var port = section.GetValue<int?>("Port");
        
        if (ipAddress == null && port == null)
            return false;
        
        endpoint = new IPEndPoint(IPAddress.Parse(ipAddress), port.Value);
        return true;
    }
}