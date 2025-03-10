using System.Net;

namespace AudioAnalyzer.Infrastructure.FileService;

public interface IFtpClient
{
    public Task<FtpWebResponse?> UploadFileToFTPServer(string uri, Stream stream);
}