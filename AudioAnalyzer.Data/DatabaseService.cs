using AudioAnalyzer.Data.Persistence.Models;
using AudioAnalyzer.Data.Persistence.Repositories;
using AudioAnalyzer.Data.Persistence.Repositories.Endpoints;
using Endpoint = AudioAnalyzer.Data.Persistence.Models.Endpoint;

namespace AudioAnalyzer.Data;

public class DatabaseService : IDisposable
{
    public readonly IRepository<AudioRequest> AudioRequestRepository;
    public readonly IRepository<User> UserRepository;
    public readonly IRepository<UploadedFile> UploadedFileRepository;
    public readonly IRepository<Endpoint> EndpointRepository;

    public DatabaseService(DataBaseContext dbContext)
    {
        AudioRequestRepository = new AudioRequestRepository(dbContext);
        UserRepository = new DbContextUserRepository(dbContext);
        UploadedFileRepository = new DbContextUploadedFileRepository(dbContext);
        EndpointRepository = new DbContextEndpointRepository(dbContext);
    }

    public async Task<AudioRequest> SaveUserRequestAsync(User user, AudioRequestType requestType, List<int> fileIds)
    {
        var endpoint = EndpointRepository.GetEntityList(e => e.EndPointType == EndPointType.FTPServer).First();
        
        AudioRequest audioRequest = new AudioRequest
        {
            AudioRequestType = requestType,
            IsProcessed = false,
            UserId = user.Id,
            FileRequestedEvents = [],
            Endpoint = endpoint
        };
        
        foreach (var fileId in fileIds)
        {
            audioRequest.FileRequestedEvents.Add(new FileRequestedEvent
            {
                AudioRequest = audioRequest,
                UploadedFileId = fileId
            });
        }
        AudioRequestRepository.Create(audioRequest);
        await AudioRequestRepository.SaveAsync();

        return audioRequest;
    }

    public async Task RollBackDbFiles(List<UploadedFile> uploadedFiles)
    {
        foreach (var file in uploadedFiles)
        {
            await UploadedFileRepository.Delete(file.Id);
        }
        await UploadedFileRepository.SaveAsync();
    }

    public void Dispose()
    {
        AudioRequestRepository.Dispose();
        UserRepository.Dispose();
        UploadedFileRepository.Dispose();
        EndpointRepository.Dispose();
    }
}
