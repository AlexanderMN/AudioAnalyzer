using System.Net;

namespace RabbitMqInfrastructure.Ftp;

public interface IFtpClient
{
    public Task<(FtpWebResponse?,long)> UploadFileToFTPServer(string uri, Stream stream);
    public Task<Stream> DownloadFileFromFTPServer(string uri);
    public Task<FtpWebResponse> CreateFtpDirectory(string uri);
    public Task<WebResponse> DeleteFile(string uri);
    public Task<FtpWebResponse> DeleteDirectory(string uri);
    public Task<FtpWebResponse> CheckIfFileExists(string uri);
    public Task<bool> CheckIfDirectoryExists(string uri);
}