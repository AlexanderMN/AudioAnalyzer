using AudioAnalyzer.Data.Persistence.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace AudioAnalyzer.Data.Persistence.Repositories;

public class DbContextUserRepository : IRepository<User>
{
    private readonly DataBaseContext _context;
    public DbContextUserRepository(DbContextOptions<DataBaseContext> options,
                                   IConfiguration configuration)
    {
        _context = new DataBaseContext(options, configuration);
    }

    public void Dispose()
    {
        _context.Dispose();   
    }

    public IEnumerable<User> GetEntityList()
    {
        return _context.Users.ToList();
    }

    public User? GetEntity(int id, bool includeRelatedEntities = false)
    {
        if (includeRelatedEntities)
        {
            return _context.Users
                           .Include(u => u.UploadedFiles)
                           .FirstOrDefault(u => u.Id == id);
        }
        
        return _context.Users.FirstOrDefault(u => u.Id == id);
    }

    public void Create(User item)
    {
        _context.Users.Add(item);
    }

    public void Update(User item)
    {
        _context.Users.Update(item);
    }

    public void Delete(int id)
    {
        var user = _context.Users.FirstOrDefault(u => u.Id == id);
        if (user == null)
            return;
        _context.Users.Remove(user);
    }

    public async Task SaveAsync()
    {
         await _context.SaveChangesAsync();
    }
}
