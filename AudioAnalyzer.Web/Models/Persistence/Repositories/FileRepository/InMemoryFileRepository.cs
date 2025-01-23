using System.Collections.Concurrent;

namespace AudioAnalyzer.Web.Models.Persistence.Repositories.FileRepository;

public class InMemoryFileRepository: IFileRepository
{
    private ConcurrentDictionary<Guid, byte[]> _files;

    public InMemoryFileRepository(ConcurrentDictionary<Guid, byte[]> files)
    {
        this._files = files;
    }

    public bool Add(Guid id, byte[] file)
    {
        return this._files.TryAdd(id, file);
    }

    public bool Delete(Guid id)
    {
        return this._files.TryRemove(id, out _);
    }

    public bool Update(Guid id, byte[] file)
    {
        if (!this._files.ContainsKey(id))
            return false;
        
        this._files[id] = file;
        return true;
    }
}