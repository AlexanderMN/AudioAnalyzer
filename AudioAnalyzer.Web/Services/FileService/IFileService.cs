using System.Net;
using Microsoft.Extensions.Primitives;

namespace AudioAnalyzer.Web.Services.FileService;

public interface IFileService
{
    public FtpWebResponse UploadFileToFTP(string uri, Stream stream);
}