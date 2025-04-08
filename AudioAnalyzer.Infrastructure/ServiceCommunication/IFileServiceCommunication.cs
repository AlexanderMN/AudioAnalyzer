using System.Net;
using AudioAnalyzer.Core;
using AudioAnalyzer.Data.Persistence.Models;

namespace AudioAnalyzer.Infrastructure.ServiceCommunication;

public interface IFileServiceCommunication
{
    public Task<Endpoint?> SendDataAsFileToFileServerAsync(User user, UploadedFile uploadedFile, Stream fileStream);
}
