using System.Net;

namespace AudioAnalyzer.Infrastructure.ServiceCommunication;

public interface IFileServiceCommunication
{
    public Task<FtpWebResponse?> SendDataToFileServiceAsync(string fileName, Stream fileStream);
}
