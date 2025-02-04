using System.Net;
using AudioAnalyzer.Infrastructure.FileService;
using AudioAnalyzer.Infrastructure.ServiceCommunication.EndpointService;

namespace AudioAnalyzer.Infrastructure.ServiceCommunication;

public class FileServiceCommunication: IFileServiceCommunication
{
    IEndpointService<string> _endpointService;
    IFileService _fileService;

    public FileServiceCommunication(IEndpointService<string> endpointService, IFileService fileService)
    {
        _endpointService = endpointService;
        _fileService = fileService;
    }

    public async Task<FtpWebResponse> SendDataToFileServiceAsync(string fileName, Stream fileStream)
    {
        var ftpResponse = await _fileService.UploadFileToFTP(
            uri: _endpointService.GetUriFromEndpointId("FTPServer", EndpointProtocol.ftp,
                                                       $"/audioFiles/{fileName}"),
            stream: fileStream);
        
        return ftpResponse;
    }
}
