using System.Net;
using AudioAnalyzer.Data.Persistence.Models;
using AudioAnalyzer.Data.Persistence.Repositories;
using AudioAnalyzer.Data.Persistence.Repositories.Endpoints;
using AudioAnalyzer.Infrastructure.ServiceCommunication;
using RabbitMqInfrastructure.Ftp;

namespace AudioAnalyzer.Infrastructure;

public class FtpStructureBuilder
{
    IFtpClient _ftpClient;
    IRepository<Endpoint> _dbEndpointRepository;

    public FtpStructureBuilder(IFtpClient ftpClient, IRepository<Endpoint> dbEndpointRepository)
    {
        _ftpClient = ftpClient;
        _dbEndpointRepository = dbEndpointRepository;
    }
    public async Task CreateDefaultFolders()
    {
        var endpoints = _dbEndpointRepository.GetEntityList(e => e.EndPointTypeId == 3);

        foreach (var endpoint in endpoints)
        {
            await CreateFolder(endpoint, "/users");
        }
    }


    public async Task CreateUserFolders(User user)
    {
        var endpoints = _dbEndpointRepository.GetEntityList(e => e.EndPointTypeId == 3);

        foreach (var endpoint in endpoints)
        {
            await CreateFolder(endpoint, $"/users/{user.Id}");
            await CreateFolder(endpoint, $"/users/{user.Id}/uploadedFiles");
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


