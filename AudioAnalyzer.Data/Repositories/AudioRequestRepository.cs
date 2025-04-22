using AudioAnalyzer.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace AudioAnalyzer.Data.Repositories;

public class AudioRequestRepository : IRepository<AudioRequest>
{
    private readonly DataBaseContext _context;

    public AudioRequestRepository(DataBaseContext context)
    {
        _context = context;
    }
    
    public void Dispose()
    {
        _context.Dispose();
    }

    public List<AudioRequest> GetEntityList(Func<AudioRequest, bool>? predicate = null)
    {
        return predicate != null ? 
            _context.AudioRequests.Where(predicate).ToList() : 
            _context.AudioRequests.ToList();
    }

    public async Task<AudioRequest?> GetEntity(int id, bool includeRelatedEntities)
    {
        if (includeRelatedEntities)
        {
            return await _context.AudioRequests
                                 .Include(ar => ar.FileRequestedEvents)
                                 .FirstOrDefaultAsync();
        }
        
        return await _context.AudioRequests.FindAsync(id);
    }

    public void Create(AudioRequest item)
    {
        _context.AudioRequests.Add(item);
    }

    public void Update(AudioRequest item)
    {
        _context.AudioRequests.Update(item);
    }

    public async Task Delete(int id)
    {
        var audioRequest = await _context.AudioRequests.FindAsync(id);
        
        if (audioRequest == null)
            return;
        _context.AudioRequests.Remove(audioRequest);
    }

    public async Task SaveAsync()
    {
        await _context.SaveChangesAsync();
    }
}
