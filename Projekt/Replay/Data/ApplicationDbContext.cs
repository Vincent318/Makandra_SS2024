using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Replay.Models;

namespace Replay.Data;

/// <summary>Contains the roles and users of the application</summary>
/// <author>Daniel Feustel</author>
public class ApplicationDbContext : IdentityDbContext<User, Role, string>
{   
    public string DbPath { get; }
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {   
        var path = Path.Join(Environment.CurrentDirectory, "persistence");
        DbPath = Path.Join(path, "app.db");
    }
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
    }
    
    protected override void OnConfiguring(DbContextOptionsBuilder options)
    => options.UseSqlite($"Data Source={DbPath}");
    
}