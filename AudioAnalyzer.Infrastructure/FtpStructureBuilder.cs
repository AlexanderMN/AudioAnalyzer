using System.Net;
using AudioAnalyzer.Data;
using AudioAnalyzer.Data.Persistence.Models;
using AudioAnalyzer.Data.Persistence.Repositories;
using AudioAnalyzer.Data.Persistence.Repositories.Endpoints;
using AudioAnalyzer.Infrastructure.ServiceCommunication;
using RabbitMqInfrastructure.Ftp;

namespace AudioAnalyzer.Infrastructure;

public class FtpStructureBuilder
{
    IFtpClient _ftpClient;
    DatabaseService _databaseService;

    public FtpStructureBuilder(IFtpClient ftpClient,
                               DatabaseService databaseService)
    {
        _ftpClient = ftpClient;
        _databaseService = databaseService;
    }
    public async Task CreateDefaultFolders()
    {
        var endpoints = _databaseService.EndpointRepository.GetEntityList(e => e.EndPointType == EndPointType.FTPServer);

        foreach (var endpoint in endpoints)
        {
            await CreateFolder(endpoint, "/users");
        }
    }


    public async Task CreateUserFolders(User user)
    {
        var endpoints = _databaseService.EndpointRepository.GetEntityList(e => e.EndPointType == EndPointType.FTPServer);

        foreach (var endpoint in endpoints)
        {
            await CreateFolder(endpoint, $"/users/{user.Id}");
            await CreateFolder(endpoint, $"/users/{user.Id}/uploadedFiles");
            await CreateFolder(endpoint, $"/users/{user.Id}/uploadFiles/split_files");
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


