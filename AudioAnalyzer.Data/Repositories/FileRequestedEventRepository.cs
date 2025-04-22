using AudioAnalyzer.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace AudioAnalyzer.Data.Repositories;

public class FileRequestedEventRepository : IRepository<FileRequestedEvent>
{
    private DataBaseContext _context;

    public FileRequestedEventRepository(DataBaseContext context)
    {
        _context = context;
    }

    public void Dispose()
    {
        _context.Dispose();
    }

    public List<FileRequestedEvent> GetEntityList(Func<FileRequestedEvent, bool>? predicate = null)
    {
        return predicate != null ? 
            _context.FileRequestedEvents.Where(predicate).ToList() : 
            _context.FileRequestedEvents.ToList();
    }

    public async Task<FileRequestedEvent?> GetEntity(int id, bool includeRelatedEntities)
    {
        if (includeRelatedEntities)
        {
            return await _context.FileRequestedEvents
                                 .Include(fre => fre.AudioResponses)
                                 .FirstOrDefaultAsync();
        }
        
        return await _context.FileRequestedEvents.FindAsync(id);
    }

    public void Create(FileRequestedEvent item)
    {
        _context.FileRequestedEvents.Add(item);
    }

    public void Update(FileRequestedEvent item)
    {
        _context.FileRequestedEvents.Update(item);
    }

    public async Task Delete(int id)
    {
        var fileRequestedEvent = await _context.FileRequestedEvents.FindAsync(id);
        
        if (fileRequestedEvent == null)
            return;
        _context.FileRequestedEvents.Remove(fileRequestedEvent);
    }

    public async Task SaveAsync()
    {
        await _context.SaveChangesAsync();
    }
}
