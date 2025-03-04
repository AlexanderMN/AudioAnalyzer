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
        var wavCrated = Database.EnsureCreated();
        
        if (wavCrated)
        {
            CreateUsers();   
        }
    }

    public DbSet<User> Users { get; set; }
    public DbSet<UploadedFile> UploadedFiles { get; set; }
    public DbSet<Endpoint> Endpoints { get; set; }
    private void CreateUsers()
    {
        var user = new User
        {
            UserName = "admin",
            Password = "admin",
            Email = "admin@admin.com",
            UploadedFiles = new Collection<UploadedFile>()
        };
        
        Users.Add(user);
        SaveChanges();
    }

    private void CreateEndpoints()
    {
        
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
