using AudioAnalyzer.Data;
using AudioAnalyzer.Infrastructure;
using AudioAnalyzer.Web.Hubs;
using Microsoft.AspNetCore.SignalR;
using RabbitMqInfrastructure.Broker;
using RabbitMqInfrastructure.Ftp;
using Endpoint = AudioAnalyzer.Data.Models.Endpoint;

namespace AudioAnalyzer.Web;

public static class InfrastructureConfiguration
{
    public static void ConfigureDatabase(IServiceScope scope)
    {
        var db = scope.ServiceProvider.GetService<DataBaseContext>();
        if (db is null)
            return;
        
        try
        {
            var created = db.Database.EnsureCreated();
            if (!created) 
                return;
            
            db.CreateEndpoints();
            db.CreateUsers();
            db.CreateUserUploadedFiles();
            db.CreateUserRequests();
            db.CreateFileRequestedEvents();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }

    public static void ConfigureFtpServer(IServiceScope scope)
    {
        using var databaseService = scope.ServiceProvider.GetRequiredService<DatabaseDbContextService>()!; 
        IFtpClient ftpClient = new FtpClient();

        var ftpStructureBuilder = new FtpStructureBuilder(
            ftpClient: ftpClient,
            databaseDbContextService: databaseService);
            
        try
        {
            ftpStructureBuilder.CreateDefaultFolders().Wait();
            ftpStructureBuilder.CreateUserFolders(databaseService.UserRepository.GetEntity(1, false).Result!).Wait();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }

    public static void ConfigureBroker(IServiceScope scope)
    {
        RabbitMqMessageConsumer consumer = new RabbitMqMessageConsumer(
            rabbitMqSetting: scope.ServiceProvider.GetService<RabbitMqSetting>()!,
            brokerQueueCallbacks: scope.ServiceProvider.GetService<BrokerQueueCallbacks>()!);
        
        consumer.StartAsync(CancellationToken.None).Wait();
    }
}
