using System.Net;
using AudioAnalyzer.Data.Persistence.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace AudioAnalyzer.Data.Persistence.Repositories.Endpoints;

public class DbContextEndpointRepository : IRepository<Endpoint>
{
    private readonly DataBaseContext _context;
    public DbContextEndpointRepository(DataBaseContext context)
    {
        _context = context;
    }


    public void Dispose()
    {
        _context.Dispose();
    }

    public List<Endpoint> GetEntityList(Func<Endpoint, bool>? predicate = null)
    {
        return predicate != null ? 
            _context.Endpoints.Where(predicate).ToList() : 
            _context.Endpoints.ToList();
    }

    public async Task<Endpoint?> GetEntity(int id, bool includeRelatedEntities)
    {
        return await _context.Endpoints.FirstOrDefaultAsync(e => e.Id == id);
    }

    public void Create(Endpoint item)
    {
        _context.Endpoints.Add(item);
    }

    public void Update(Endpoint item)
    {
        _context.Endpoints.Update(item);
    }

    public async Task Delete(int id)
    {
        var endpoint = await _context.Endpoints.FindAsync(id);
        if (endpoint == null)
            return;
        _context.Endpoints.Remove(endpoint);
    }

    public async Task SaveAsync()
    {
        await _context.SaveChangesAsync();
    }
}
