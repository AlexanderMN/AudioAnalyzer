using System.Collections.ObjectModel;
using AudioAnalyzer.Data.Models;
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

    public DataBaseContext(IConfiguration config)
    {
        _configuration = config;
    }
    
    public DbSet<User> Users { get; set; }
    public DbSet<UploadedFile> UploadedFiles { get; set; }
    public DbSet<Endpoint> Endpoints { get; set; }
    public DbSet<AudioRequest> AudioRequests { get; set; }
    public DbSet<FileRequestedEvent> FileRequestedEvents { get; set; }
    public DbSet<AudioResponse> AudioResponses { get; set; }
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
    public void CreateEndpoints()
    {
        List<Endpoint> endpoints =
        [
            new Endpoint
            {
                Name = "ftp_1",
                IPAddress = "192.168.93.156",
                Port = 21,
                Active = true,
                Username = "alexMN",
                Password = "3217AlexN",
                EndPointType = EndPointType.FTPServer
            },
            new Endpoint
            {
                Name = "rabbit_1",
                IPAddress = "192.168.93.156",
                Port = 5672,
                Active = true,
                Username = "guest",
                Password = "guest",
                EndPointType = EndPointType.Broker
            }
        ];
        Endpoints.AddRange(endpoints);
        SaveChanges();
    }

    public void CreateUserUploadedFiles()
    {
        var uploadedFile = new UploadedFile
        {
            UploadedFileName = "startup",
            UploadedFileType = "wav",
            FileState = FileState.Ready,
            Duration = 322,
            SplitNumber = 2,
            UploadedDate = DateTime.UtcNow,
            UserId = 1,
            EndpointId = 1
        };
        UploadedFiles.Add(uploadedFile);
        SaveChanges();
    }

    public void CreateUserRequests()
    {
        var request = new AudioRequest
        {
            State = AudioRequestState.Processed,
            AudioRequestType = AudioRequestType.Transcribe,
            UserId = 1,
            EndpointId = 1,
            CreationDate = DateTime.UtcNow,
        };
        AudioRequests.Add(request);
        SaveChanges();
    }

    public void CreateFileRequestedEvents()
    {
        var fre = new FileRequestedEvent
        {
            AudioRequestId = 1,
            UploadedFileId = 1,
        };
        FileRequestedEvents.Add(fre);
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
        modelBuilder.Entity<FileRequestedEvent>().HasIndex(fre => new { fre.UploadedFileId, fre.AudioRequestId })
                    .IsUnique();

        modelBuilder.Entity<AudioResponse>().HasIndex(ar => new { ar.OrderId, ar.FileRequestedEventId });
        modelBuilder.Entity<FileRequestedEvent>().HasIndex(fre => new { fre.UploadedFileId, fre.AudioRequestId });
        
        modelBuilder.Entity<AudioRequest>()
                    .Property(ar => ar.AudioRequestType)
                    .HasConversion<string>();
        
        modelBuilder.Entity<AudioRequest>()
                    .Property(ar => ar.State)
                    .HasConversion<string>();
        
        modelBuilder.Entity<Endpoint>()
                    .Property(ar => ar.EndPointType)
                    .HasConversion<string>();
        
        modelBuilder.Entity<AudioResponse>()
                    .Property(ar => ar.ResponseType)
                    .HasConversion<string>();
        
        base.OnModelCreating(modelBuilder);
    }
}
