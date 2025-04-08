using System.Collections.ObjectModel;
using AudioAnalyzer.Data.Persistence.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace AudioAnalyzer.Data;

public sealed class DataBaseContext : DbContext
{
    IConfiguration _configuration;
    
    public DataBaseContext(DbContextOptions options, IConfiguration config) : base(options)
    {
        _configuration = config;
    }

    public DbSet<EndPointType> EndPointTypes { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<UploadedFile> UploadedFiles { get; set; }
    public DbSet<Endpoint> Endpoints { get; set; }
    public DbSet<AudioRequest> AudioRequests { get; set; }
    public DbSet<AudioRequestType> AudioRequestTypes { get; set; }
    public DbSet<FileRequestedEvent> FileRequestedEvents { get; set; }
    public void CreateUsers()
    {
        var user = new User
        {
            UserName = "admin",
            Password = "admin",
            Email = "admin@admin.com"
        };
        
        Users.Add(user);
        SaveChanges();
    }

    public void CreateEndpointTypes()
    {
        List<EndPointType> endpointTypes =
        [
            new EndPointType { Name = "AudioRecognizer" },
            new EndPointType { Name = "Broker" },
            new EndPointType { Name = "FTPServer" }
        ];
        
        EndPointTypes.AddRange(endpointTypes);
        SaveChanges();
    }
    public void CreateEndpoints()
    {
        var endpoint = new Endpoint
        {
            Name = "ftp_1",
            IPAddress = "127.0.0.1",
            Port = 21,
            Username = "alexMN",
            Password = "3217AlexN",
            EndPointTypeId = 3
        };
        Endpoints.Add(endpoint);
        SaveChanges();
    }

    public void CreateAudioRequestTypes()
    {
        List<AudioRequestType> requestTypes =
        [
            new AudioRequestType { Name = "Transcribe" },
            new AudioRequestType { Name = "Search" },
            new AudioRequestType { Name = "Summarize" },
        ];
        
        AudioRequestTypes.AddRange(requestTypes);
        SaveChanges();
    }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(_configuration.GetConnectionString("DefaultConnection"));
        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>().HasIndex(u => u.UserName).IsUnique();
        
        base.OnModelCreating(modelBuilder);
    }
}
