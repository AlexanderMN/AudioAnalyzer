using System.Net;
using AudioAnalyzer.Infrastructure.FileService;
using AudioAnalyzer.Infrastructure.ServiceCommunication.EndpointService;

namespace AudioAnalyzer.Infrastructure.ServiceCommunication;

public class FileServiceCommunication: IFileServiceCommunication
{
    IEndpointService<string> _endpointService;
    IFtpClient _ftpClient;

    public FileServiceCommunication(IEndpointService<string> endpointService, IFtpClient ftpClient)
    {
        _endpointService = endpointService;
        _ftpClient = ftpClient;
    }

    public async Task<FtpWebResponse?> SendDataToFileServiceAsync(string fileName, Stream fileStream)
    {
        var ftpResponse = await _ftpClient.UploadFileToFTPServer(
            uri: _endpointService.GetUriFromEndpointId("FTPServer", EndpointProtocol.ftp,
                                                       $"/audioFiles/{fileName}"),
            stream: fileStream);
        
        return ftpResponse;
    }
}
