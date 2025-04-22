using AudioAnalyzer.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace AudioAnalyzer.Data.Repositories;

public class DbContextUserRepository : IRepository<User>
{
    private readonly DataBaseContext _context;
    public DbContextUserRepository(DataBaseContext context)
    {
        _context = context;
    }

    public void Dispose()
    {
        _context.Dispose();   
    }

    public List<User> GetEntityList(Func<User, bool>? predicate = null)
    {
        return predicate != null ? 
            _context.Users.Where(predicate).ToList() : 
            _context.Users.ToList();
    }

    public async Task<User?> GetEntity(int id, bool includeRelatedEntities = false)
    {
        if (includeRelatedEntities)
        {
            return await _context.Users
                           .Include(u => u.Requests)
                           .ThenInclude(r => r.FileRequestedEvents)
                           .FirstOrDefaultAsync(u => u.Id == id, CancellationToken.None);
        }
        
        return await _context.Users.FindAsync(id);
    }

    public void Create(User item)
    {
        _context.Users.Add(item);
    }

    public void Update(User item)
    {
        _context.Users.Update(item);
    }

    public async Task Delete(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null)
            return;
        _context.Users.Remove(user);
    }

    public async Task SaveAsync()
    {
         await _context.SaveChangesAsync();
    }
}
