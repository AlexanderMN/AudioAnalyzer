using System.Net;
using AudioAnalyzer.Core;
using AudioAnalyzer.Data.Models;
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
        await fileStream.CopyToAsync(wavFileStream);
        await wavFileStream.FlushAsync();
        wavFileStream.Position = 0;
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
    public async Task<MemoryStream?> GetDataFromFileServerAsync(UploadedFile file)
    {
        var fileStream = await _ftpClient.DownloadFileFromFTPServer(
            uri: EndpointService.GetEndpointUri(
                endpoint: file.Endpoint,
                endpointProtocol: EndpointProtocol.ftp,
                $"/users/{file.UserId}/{FtpSettings.DefaultFileUploadFolder}/{file.Id}.{file.UploadedFileType}"));
        
        MemoryStream memoryStream = new MemoryStream();
        await fileStream.CopyToAsync(memoryStream);
        await memoryStream.FlushAsync();
        return memoryStream;
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
