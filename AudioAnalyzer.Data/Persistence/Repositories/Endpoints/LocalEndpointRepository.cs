using System.Net;
using AudioAnalyzer.Data.Persistence.Models;
using Microsoft.Extensions.Configuration;

namespace AudioAnalyzer.Data.Persistence.Repositories.Endpoints;

public class LocalEndpointRepository : IRepository<Endpoint> 
{
    private IConfiguration _configuration;

    public LocalEndpointRepository(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public IPEndPoint GetEndpoint(string id)
    {
        var sectionId = id;

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

    public void Dispose()
    {
        _configuration = null;
    }

    public IEnumerable<Endpoint> GetEntityList()
    {
        var endpoints = new List<Endpoint>();
        
        var remoteEndpointSections = GetRemoteEndpointsSection().GetChildren();
        
        foreach (var remoteEndpointSection in remoteEndpointSections)
        {
            
            if (TryExtractEndpointFromSction(remoteEndpointSection, out IPEndPoint endpoint))
            {

            }
            
        }
        
        return endpoints;
    }

    public List<Endpoint> GetEntityList(Func<Endpoint, bool>? predicate = null)
    {
        throw new NotImplementedException();
    }

    public Task<Endpoint?> GetEntity(int id, bool includeRelatedEntities)
    {
        throw new NotImplementedException();
    }

    public void Create(Endpoint item)
    {
        throw new NotImplementedException();
    }

    public void Update(Endpoint item)
    {
        throw new NotImplementedException();
    }

    public async Task Delete(int id)
    {
        throw new NotImplementedException();
    }

    public Task SaveAsync()
    {
        throw new NotImplementedException();
    }
}