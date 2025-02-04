using System.Net;
using Microsoft.Extensions.Configuration;

namespace AudioAnalyzer.Data.Persistence.Repositories.Endpoints;

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
        var sectionId = id as string;

        if (sectionId == null)
            return null;

        var endpointSection = GetRemoteEndpointsSection().GetSection(sectionId);
        TryExtractEndpointFromSction(endpointSection, out IPEndPoint endpoint);    
        
        return endpoint;
    }

    private IConfigurationSection GetRemoteEndpointsSection()
    {
        return _configuration.GetSection("RemoteEndpoints");
    }

    private bool TryExtractEndpointFromSction(IConfigurationSection section, out IPEndPoint endpoint)
    {
        endpoint = null;
        var ipAddress = section["IPAddress"];
        
        if (string.IsNullOrEmpty(ipAddress))
            return false;
        
        if (Int32.TryParse(section["Port"], out int port))
        {
            endpoint = new IPEndPoint(IPAddress.Parse(ipAddress), port);
            return true;    
        }
        
        return false;
    }
}