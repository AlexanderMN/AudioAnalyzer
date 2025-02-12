using AudioAnalyzer.Data.Persistence.Models;

namespace AudioAnalyzer.Infrastructure.FileService;

public class FileRepo
{
    public HashSet<User> activeUsers = new();
}
