using System.Net;

namespace AudioAnalyzer.Infrastructure.FileService;

public interface IFileService
{
    public FtpWebResponse UploadFileToFTP(string uri, Stream stream);
}