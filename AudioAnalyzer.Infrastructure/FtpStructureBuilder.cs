using System.Net;
using AudioAnalyzer.Data;
using AudioAnalyzer.Data.Models;
using AudioAnalyzer.Infrastructure.ServiceCommunication;
using RabbitMqInfrastructure.Ftp;

namespace AudioAnalyzer.Infrastructure;

public class FtpStructureBuilder
{
    IFtpClient _ftpClient;
    DatabaseDbContextService _databaseDbContextService;

    public FtpStructureBuilder(IFtpClient ftpClient,
                               DatabaseDbContextService databaseDbContextService)
    {
        _ftpClient = ftpClient;
        _databaseDbContextService = databaseDbContextService;
    }
    public async Task CreateDefaultFolders()
    {
        var endpoints = _databaseDbContextService.EndpointRepository.GetEntityList(e => e.EndPointType == EndPointType.FTPServer);

        foreach (var endpoint in endpoints)
        {
            await CreateFolder(endpoint, "/users");
        }
    }


    public async Task CreateUserFolders(User user)
    {
        var endpoints = _databaseDbContextService.EndpointRepository.GetEntityList(e => e.EndPointType == EndPointType.FTPServer);

        foreach (var endpoint in endpoints)
        {
            await CreateFolder(endpoint, $"/users/{user.Id}");
            await CreateFolder(endpoint, $"/users/{user.Id}/uploadedFiles");
            await CreateFolder(endpoint, $"/users/{user.Id}/uploadedFiles/split_files");
            await CreateFolder(endpoint, $"/users/{user.Id}/requests");
        }
    }
    private async Task CreateFolder(Endpoint endpoint, string folderPath)
    {
        var directoryUri = EndpointService.GetEndpointUri(
            endpoint: endpoint,
            endpointProtocol: EndpointProtocol.ftp,
            internalPath: folderPath);
        var directoryExists = await _ftpClient.CheckIfDirectoryExists(directoryUri);

        if (!directoryExists)
        {
            await _ftpClient.CreateFtpDirectory(uri: directoryUri);   
        }
    }
}


