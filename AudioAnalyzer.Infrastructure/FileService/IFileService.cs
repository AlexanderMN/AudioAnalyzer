using System.Net;

namespace AudioAnalyzer.Infrastructure.FileService;

public interface IFileService
{
    public Task<FtpWebResponse> UploadFileToFTP(string uri, Stream stream);
}