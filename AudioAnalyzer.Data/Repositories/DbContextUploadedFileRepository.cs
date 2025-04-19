using AudioAnalyzer.Data.Persistence.Models;
using Microsoft.EntityFrameworkCore;

namespace AudioAnalyzer.Data.Persistence.Repositories;

public class DbContextUploadedFileRepository : IRepository<UploadedFile>
{
    private readonly DataBaseContext _context;

    public DbContextUploadedFileRepository(DataBaseContext context)
    {
        _context = context;
    }

    public void Dispose()
    {
        _context.Dispose();
    }

    public List<UploadedFile> GetEntityList(Func<UploadedFile, bool>? predicate = null)
    {
        return predicate != null ? 
            _context.UploadedFiles.Where(predicate).ToList() : 
            _context.UploadedFiles.ToList();
    }

    public async Task<UploadedFile?> GetEntity(int id, bool includeRelatedEntities)
    {
        return await _context.UploadedFiles.FirstOrDefaultAsync(e => e.Id == id);
    }

    public void Create(UploadedFile item)
    {
        _context.UploadedFiles.Add(item);
    }

    public void Update(UploadedFile item)
    {
        _context.UploadedFiles.Update(item);
    }

    public async Task Delete(int id)
    {
        var uploadedFile = await _context.UploadedFiles.FindAsync(id);
        if (uploadedFile == null)
            return;
        _context.UploadedFiles.Remove(uploadedFile);
    }

    public async Task SaveAsync()
    {
        await _context.SaveChangesAsync();
    }
}
