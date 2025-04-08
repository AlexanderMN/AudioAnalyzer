using AudioAnalyzer.Data.Persistence.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace AudioAnalyzer.Data.Persistence.Repositories.Endpoints;

public class DbContextEndpointTypeRepository : IRepository<EndPointType>
{
    private readonly DataBaseContext _dbContext;

    public DbContextEndpointTypeRepository(DataBaseContext dbContext)
    {
        _dbContext = dbContext;
    }

    public void Dispose()
    {
        _dbContext.Dispose();
    }

    public List<EndPointType> GetEntityList(Func<EndPointType, bool>? predicate = null)
    {
        return predicate != null ? 
            _dbContext.EndPointTypes.Where(predicate).ToList() : 
            _dbContext.EndPointTypes.ToList();
    }

    public async Task<EndPointType?> GetEntity(int id, bool includeRelatedEntities)
    {
        return await _dbContext.EndPointTypes.FindAsync(id);
    }

    public void Create(EndPointType item)
    {
        _dbContext.EndPointTypes.Add(item);
    }

    public void Update(EndPointType item)
    {
        _dbContext.EndPointTypes.Update(item);
    }

    public async Task Delete(int id)
    {
        var endpoint = await _dbContext.EndPointTypes.FindAsync(id);
        if (endpoint == null)
            return;
        _dbContext.EndPointTypes.Remove(endpoint);
    }

    public async Task SaveAsync()
    {
        await _dbContext.SaveChangesAsync();
    }
}
