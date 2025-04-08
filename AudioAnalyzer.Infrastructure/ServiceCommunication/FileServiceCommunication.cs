using System.Net;
using AudioAnalyzer.Core;
using AudioAnalyzer.Data.Persistence.Models;
using RabbitMqInfrastructure.Ftp;

namespace AudioAnalyzer.Infrastructure.ServiceCommunication;

public class FileServiceCommunication: IFileServiceCommunication
{
    private int ftpEndpointTypeId = 3; // TODO: create endpoint id synchronization mechanism
    private readonly EndpointService _endpointService;
    private readonly IFtpClient _ftpClient;

    public FileServiceCommunication(EndpointService endpointService, IFtpClient ftpClient)
    {
        _endpointService = endpointService;
        _ftpClient = ftpClient;
    }

    public async Task<Endpoint?> SendDataAsFileToFileServerAsync(User user, UploadedFile uploadedFile, Stream fileStream)
    {
        var endPoint = _endpointService.GetAvailableEndpoint(ftpEndpointTypeId);
        
        if (endPoint == null)
            return null;

        var ftpResponse = await _ftpClient.CreateFtpDirectory(
            uri: EndpointService.GetEndpointUri(endpoint: endPoint,
                                                 endpointProtocol: EndpointProtocol.ftp,
                                                 internalPath: GetUserUploadFolderPath(user, uploadedFile)));

        if (ftpResponse.StatusCode != FtpStatusCode.PathnameCreated)
            return null;
        
        ftpResponse.Dispose();
        
        MemoryStream wavFileStream = new MemoryStream();
        
        if (!await FfMpegWrapper.ConvertStreamToStream(
            inputStream: fileStream,
            outputStream: wavFileStream,
            outputFormat: SupportedAudioFormats.Wav))
            return null;
        
        uploadedFile.UploadedFileType = SupportedAudioFormats.Wav;
        var uploadResult = await _ftpClient.UploadFileToFTPServer(
            uri: EndpointService.GetEndpointUri(endpoint: endPoint,
                                                 endpointProtocol: EndpointProtocol.ftp,
                                                 internalPath: GetUserInternalFilePath(user, uploadedFile)),
            stream: wavFileStream);
        
        return uploadResult.Item1?.StatusCode != FtpStatusCode.ClosingData ? null: endPoint;
    }
    public async Task<Stream?> GetDataFromFileServerAsync(UploadedFile file)
    {
        var endpoint = await _endpointService.GetEndpoint(
            endPointId: file.EndpointId);
        
        if (endpoint == null)
            return null;
        
        var fileStream = await _ftpClient.DownloadFileFromFTPServer(
            uri: EndpointService.GetEndpointUri(
                endpoint: endpoint,
                endpointProtocol: EndpointProtocol.ftp,
                $"/{FtpSettings.DefaultFileDownloadFolder}/{file.UploadedFileName}.{file.UploadedFileType}"));
        
        return fileStream;
    }

    public static string GetUserUploadFolderPath(User user, UploadedFile uploadedFile)
    {
        return $"/users/{user.Id}/{FtpSettings.DefaultFileUploadFolder}/{uploadedFile.Id}";
    }
    
    public static string GetUserInternalFilePath(User user,UploadedFile uploadedFile)
    {
        return $"/users/{user.Id}/{FtpSettings.DefaultFileUploadFolder}/{uploadedFile.Id}.{uploadedFile.UploadedFileType}";
    }
}
