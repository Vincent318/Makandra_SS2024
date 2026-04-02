
using Microsoft.EntityFrameworkCore;
using Replay.Models;

namespace Replay.Data;
/// <author>Daniel Feustel</author>
public class SecurityDbContext : DbContext
{   
    public virtual DbSet<SecurityModel> SecurityDB {get; set;}
    public string DbPath { get; }
    public SecurityDbContext()
    {   
        var path = Path.Join(Environment.CurrentDirectory, "persistence");
        DbPath = Path.Join(path, "security.db");
        
    }
    
    protected override void OnConfiguring(DbContextOptionsBuilder options)
    => options.UseSqlite($"Data Source={DbPath}");
    
}