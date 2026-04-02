using Microsoft.EntityFrameworkCore;
using Replay.Models;

namespace Replay.Data
{
    /// <summary>
    /// Handles the database for the Operation-Repository
    /// </summary>
    /// <author>Raphael Huber</author>
    public class OperationDBContext : DbContext
    {
        /// <summary>A set for operations</summary>
        /// <author>Raphael Huber</author>   
        public virtual DbSet<Operation> Operations {get; set;}

        /// <summary>The path where the database should be stored</summary>
        /// <author>Raphael Huber</author>  
        public string DbPath { get; }

        /// <summary>
        /// sets the DbPath to Replay/persistence/operations.db
        /// </summary>
        /// <author>Raphael Huber</author>
        public OperationDBContext()
        {
            var path = Path.Join(Environment.CurrentDirectory, "persistence");
            DbPath = Path.Join(path, "operations.db");
        }

        /// <summary>
        /// Defines that the context should use a sqlite-database in the DbPath whenever a new context is created
        /// </summary>
        /// <param name="options">instance of DbContextOptionsBuilder that is used to change the database</param>
        /// <author>Raphael Huber</author>
        protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlite($"Data Source={DbPath}");
    
        /// <summary>
        /// Overrides the model-creation to use a 1:n-relationship between operations and tasks
        /// </summary>
        /// <param name="modelBuilder">instance of ModelBuilder that is used to define the model</param>
        /// <author>Raphael Huber</author>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Operation>()
            .HasMany(e => e.Tasks)
            .WithOne(e => e.Operation)
            .HasForeignKey(e => e.OperationId)
            .IsRequired(); //the task can only exists whenever a operation exists so the foreign-key is required
        }
    }
}
