using System.Net;
using AudioAnalyzer.Core;
using AudioAnalyzer.Data.Persistence.Models;
using RabbitMqInfrastructure.Ftp;

namespace AudioAnalyzer.Infrastructure.ServiceCommunication;

public class FileServiceCommunication
{
    private int ftpEndpointTypeId = 3; // TODO: create endpoint id synchronization mechanism
    private readonly IFtpClient _ftpClient;

    public FileServiceCommunication(IFtpClient ftpClient)
    {
        _ftpClient = ftpClient;
    }

    public async Task<bool> SendDataAsFileToFileServerAsync(User user, 
                                                                 UploadedFile uploadedFile,
                                                                 Stream fileStream)
    {
        
        MemoryStream wavFileStream = new MemoryStream();
        if (!await FfMpegWrapper.ConvertStreamToStream(
            inputStream: fileStream,
            outputStream: wavFileStream,
            outputFormat: SupportedAudioFormats.Wav))
            return false;
        
        uploadedFile.UploadedFileType = SupportedAudioFormats.Wav;
        var uploadResult = await _ftpClient.UploadFileToFTPServer(
            uri: EndpointService.GetEndpointUri(endpoint: uploadedFile.Endpoint,
                                                 endpointProtocol: EndpointProtocol.ftp,
                                                 internalPath: GetUserInternalFilePath(user, uploadedFile)),
            stream: wavFileStream);
        
        return uploadResult.Item1?.StatusCode == FtpStatusCode.ClosingData;
    }

    public async Task<bool> CreateRequestFolder(AudioRequest request)
    {
        var webResponse = await _ftpClient.CreateFtpDirectory(
            EndpointService.GetEndpointUri(
                endpoint: request.Endpoint,
                endpointProtocol: EndpointProtocol.ftp,
                $"/users/{request.UserId}/{FtpSettings.DefaultAudioRequestFolder}/{request.Id}")
        );

        return webResponse.StatusCode == FtpStatusCode.PathnameCreated;
    }
    public async Task<Stream?> GetDataFromFileServerAsync(Endpoint endpoint, UploadedFile file)
    {
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
