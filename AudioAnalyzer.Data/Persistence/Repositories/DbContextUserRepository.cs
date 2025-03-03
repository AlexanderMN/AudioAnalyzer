using AudioAnalyzer.Data.Persistence.Models;
using Microsoft.EntityFrameworkCore;

namespace AudioAnalyzer.Data.Persistence.Repositories;

public class DbContextUserRepository : IRepository<User>
{
    private readonly DataBaseContext _context;

    public DbContextUserRepository(DataBaseContext context)
    {
        _context = context;
    }

    public void Dispose()
    {
        
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
                           .FirstOrDefault(u => u.UserId == id);
        }
        
        return _context.Users.FirstOrDefault(u => u.UserId == id);
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
        var user = _context.Users.FirstOrDefault(u => u.UserId == id);
        if (user == null)
            return;
        _context.Users.Remove(user);
    }

    public async Task SaveAsync()
    {
         await _context.SaveChangesAsync();
    }
}
