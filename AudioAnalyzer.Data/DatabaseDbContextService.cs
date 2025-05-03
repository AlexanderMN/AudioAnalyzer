using AudioAnalyzer.Data.Models;
using AudioAnalyzer.Data.Repositories;
using AudioAnalyzer.Data.Repositories.Endpoints;
using Microsoft.EntityFrameworkCore;
using Endpoint = AudioAnalyzer.Data.Models.Endpoint;

namespace AudioAnalyzer.Data;

public class DatabaseDbContextService : IDisposable
{
    private readonly DataBaseContext _context;
    
    public readonly IRepository<AudioRequest> AudioRequestRepository;
    public readonly IRepository<User> UserRepository;
    public readonly IRepository<UploadedFile> UploadedFileRepository;
    public readonly IRepository<Endpoint> EndpointRepository;
    public readonly IRepository<AudioResponse> AudioResponseRepository;
    public readonly IRepository<FileRequestedEvent> FileRequestedEventRepository;
    public DatabaseDbContextService(DataBaseContext dbContext)
    {
        _context = dbContext;
        AudioRequestRepository = new AudioRequestRepository(dbContext);
        UserRepository = new DbContextUserRepository(dbContext);
        UploadedFileRepository = new DbContextUploadedFileRepository(dbContext);
        EndpointRepository = new DbContextEndpointRepository(dbContext);
        AudioResponseRepository = new AudioResponseRepository(dbContext);
        FileRequestedEventRepository = new FileRequestedEventRepository(dbContext);
    }

    public async Task<AudioRequest> SaveUserRequestAsync(User user, AudioRequestType requestType, List<int> fileIds)
    {
        var endpoint = EndpointRepository.GetEntityList(e => e.EndPointType == EndPointType.FTPServer).First();
        
        AudioRequest audioRequest = new AudioRequest
        {
            AudioRequestType = requestType,
            State = AudioRequestState.Processing,
            UserId = user.Id,
            FileRequestedEvents = [],
            CreationDate = DateTime.UtcNow,
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

    public async Task<FileRequestedEvent?> GetFileRequestedEventByIndex(int requestId, int fileId, bool includeResponses = false)
    {
        if (includeResponses)
            return await _context.FileRequestedEvents
                                 .Include(fre => fre.UploadedFile)
                                 .Include(fre => fre.AudioRequest)
                                 .Include(fre => fre.AudioResponses)
                                 .FirstOrDefaultAsync(fre => fre.UploadedFileId == fileId &&
                                                             fre.AudioRequestId == requestId);
        
        
        return await _context.FileRequestedEvents
                             .Include(fre => fre.UploadedFile)
                             .Include(fre => fre.AudioRequest)
                             .FirstOrDefaultAsync(fre => fre.UploadedFileId == fileId &&
                                                  fre.AudioRequestId == requestId);
    }
    public void Dispose()
    {
        AudioRequestRepository.Dispose();
        UserRepository.Dispose();
        UploadedFileRepository.Dispose();
        EndpointRepository.Dispose();
    }

    public async Task SetFileRequestedEventState(FileRequestedEvent fileRequestedEvent)
    {
        var audioResponses = AudioResponseRepository
                                                      .GetEntityList(ar => ar.FileRequestedEventId == fileRequestedEvent.Id);

        if (audioResponses.All(ar => ar.ResponseType == AudioResponseType.Success))
        {
            fileRequestedEvent.State = FileRequestedEventState.Completed;
            FileRequestedEventRepository.Update(fileRequestedEvent);
            await FileRequestedEventRepository.SaveAsync();
            return;
        }

        if (audioResponses.All(ar => ar.ResponseType == AudioResponseType.Error))
        {
            fileRequestedEvent.State = FileRequestedEventState.Failed;
            FileRequestedEventRepository.Update(fileRequestedEvent);
            await FileRequestedEventRepository.SaveAsync();
            return;
        }
            
        fileRequestedEvent.State = FileRequestedEventState.CompletedWithError;
        FileRequestedEventRepository.Update(fileRequestedEvent);
        await FileRequestedEventRepository.SaveAsync();
    }
}
