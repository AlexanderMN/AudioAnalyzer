using System.Text.Json;
using System.Text.Json.Serialization;
using AudioAnalyzer.Data;
using AudioAnalyzer.Data.Models;
using Microsoft.Extensions.Configuration;

namespace AudioAnalyzer.Scaler;

class Program
{
    static async Task Main(string[] args)
    {
        DataBaseContext dataBaseContext = new DataBaseContext();
        DatabaseDbContextService databaseDbContextService = new DatabaseDbContextService(dataBaseContext);
        
        Console.WriteLine("Enter config file path:");
        string? path = Console.ReadLine();
        List<Endpoint> endpoints;
        using (StreamReader sr = new StreamReader(path))
        {
            string json = await sr.ReadToEndAsync();
            endpoints = JsonSerializer.Deserialize<List<Endpoint>>(json);
        }

        Console.WriteLine("Enter action:");
        Console.WriteLine("Add:0    Update:1    Delete:2");
        UserActions userActions = (UserActions)Int32.Parse(Console.ReadLine());


        try
        {
            switch (userActions)
            {
                case UserActions.Add:
                    foreach (var endpoint in endpoints)
                    {
                        databaseDbContextService.EndpointRepository.Create(endpoint);
                    }
                    await databaseDbContextService.EndpointRepository.SaveAsync();
                    break;
            
                case UserActions.Update:
                    foreach (var endpoint in endpoints)
                    {
                        databaseDbContextService.EndpointRepository.Update(endpoint);
                    }
                    await databaseDbContextService.EndpointRepository.SaveAsync();
                    break;
            
                case UserActions.Delete:
                    foreach (var endpoint in endpoints)
                    {
                        await databaseDbContextService.EndpointRepository.Delete(endpoint.Id);
                    }
                    await databaseDbContextService.EndpointRepository.SaveAsync();
                    break;
            }

            Console.WriteLine("Done");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    private enum UserActions 
    {
        Add,
        Update,
        Delete
    }
}
