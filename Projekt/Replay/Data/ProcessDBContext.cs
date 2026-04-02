// @author: Vincent Arnold
using Microsoft.EntityFrameworkCore;
using Replay.Models;
using System.IO;

namespace Replay.Data
{
    /// <summary>
    /// Context class for interacting with the database using Entity Framework Core.
    /// </summary>
    public class ProcessDBContext : DbContext
    {
        // DbSet representing the collection of Process entities in the database
        public virtual DbSet<Process> Processes { get; set; }

        // Path to the database file
        public string dbpath { get; }

        // Constructor accepting DbContextOptions and setting up the database file path
        public ProcessDBContext(DbContextOptions<ProcessDBContext> options)
            : base(options)
        {
            // Define the path for the database file
            var path = Path.Join(Environment.CurrentDirectory, "persistence");
            dbpath = Path.Join(path, "process.db");
        }

        // Configures the context to use SQLite with the specified database file path
        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite($"Data Source={dbpath}");
        
        // Configures the entity relationships and database schema
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Define the relationship between Process and Task entities
            modelBuilder.Entity<Process>()
                .HasMany(p => p.Tasks)         // A Process has many Tasks
                .WithOne(t => t.Process)       // Each Task is associated with one Process
                .HasForeignKey(t => t.ProcessId); // Foreign key for the relationship
        }
    }
}