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
        
        var created = Database.EnsureCreated();
        if (!created) 
            return;
        
        CreateEndpointTypes();
        CreateEndpoints();
        CreateUsers();
    }
    
    public DataBaseContext(IConfiguration config)
    {
        _configuration = config;
        
        var created = Database.EnsureCreated();
        if (!created) 
            return;
        
        CreateEndpointTypes();
        CreateEndpoints();
        CreateUsers();
    }

    public DbSet<EndPointType> EndPointTypes { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<UploadedFile> UploadedFiles { get; set; }
    public DbSet<Endpoint> Endpoints { get; set; }
    public DbSet<AudioRequest> AudioRequests { get; set; }
    public DbSet<AudioRequestType> AudioRequestTypes { get; set; }
    public DbSet<FileRequestedEvent> FileRequestedEvents { get; set; }
    private void CreateUsers()
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

    private void CreateEndpointTypes()
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
    private void CreateEndpoints()
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

    private void CreateAudioRequestTypes()
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
