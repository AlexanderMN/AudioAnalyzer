using System.Collections.ObjectModel;
using AudioAnalyzer.Data.Persistence.Models;
using Microsoft.EntityFrameworkCore;

namespace AudioAnalyzer.Data;

public class DataBaseContext : DbContext
{
    public DataBaseContext(DbContextOptions options) : base(options)
    {
        CreateUsers();   
    }

    public DbSet<User> Users { get; set; }
    public DbSet<UploadedFile> UploadedFiles { get; set; }

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
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>().HasIndex(u => u.UserName).IsUnique();
        
        base.OnModelCreating(modelBuilder);
    }
}
