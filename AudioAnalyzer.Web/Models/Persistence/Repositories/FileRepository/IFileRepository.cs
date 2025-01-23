namespace AudioAnalyzer.Web.Models.Persistence.Repositories.FileRepository;

public interface IFileRepository
{
    public bool Add(Guid id, byte[] file);
    public bool Delete(Guid id);
    public bool Update(Guid id, byte[] file);
}