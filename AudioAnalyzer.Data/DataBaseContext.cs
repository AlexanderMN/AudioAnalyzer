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
        var endpoint = new Endpoint
        {
            Name = "ftp_1",
            IPAddress = "127.0.0.1",
            Port = 21,
            Username = "alexMN",
            Password = "3217AlexN",
            EndPointType = EndPointType.FTPServer
        };
        Endpoints.Add(endpoint);
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
        
        modelBuilder.Entity<Endpoint>()
                    .Property(ar => ar.EndPointType)
                    .HasConversion<string>();
        
        modelBuilder.Entity<AudioResponse>()
                    .Property(ar => ar.ResponseType)
                    .HasConversion<string>();
        
        base.OnModelCreating(modelBuilder);
    }
}
