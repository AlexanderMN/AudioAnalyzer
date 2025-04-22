using AudioAnalyzer.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace AudioAnalyzer.Data.Repositories;

public class AudioResponseRepository : IRepository<AudioResponse>
{
    private readonly DataBaseContext _context;

    public AudioResponseRepository(DataBaseContext context)
    {
        _context = context;
    }

    public void Dispose()
    {
        _context.Dispose();
    }

    public List<AudioResponse> GetEntityList(Func<AudioResponse, bool>? predicate = null)
    {
        return predicate != null ? 
            _context.AudioResponses.Where(predicate).ToList() : 
            _context.AudioResponses.ToList();
    }

    public async Task<AudioResponse?> GetEntity(int id, bool includeRelatedEntities)
    {
        if (includeRelatedEntities)
        {
            return await _context.AudioResponses
                                 .Include(ar => ar.FileRequestedEvent)
                                 .FirstOrDefaultAsync();
        }
        
        return await _context.AudioResponses.FindAsync(id);
    }

    public void Create(AudioResponse item)
    {
        _context.AudioResponses.Add(item);
    }

    public void Update(AudioResponse item)
    {
        _context.AudioResponses.Update(item);
    }

    public async Task Delete(int id)
    {
        var audioResponse = await _context.AudioResponses.FindAsync(id);
        
        if (audioResponse == null)
            return;
        _context.AudioResponses.Remove(audioResponse);
    }

    public async Task SaveAsync()
    {
        await _context.SaveChangesAsync();
    }
}
