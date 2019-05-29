using Microsoft.EntityFrameworkCore;
using Tolldo.Models;

namespace Tolldo.Data
{
    /// <summary>
    /// The database context for the application 'Tolldo'.
    /// Uses EntityFramework and SQLite.
    /// </summary>
    public class TolldoDbContext : DbContext
    {
        #region DbSets

        public DbSet<Todo> Todos { get; set; }
        public DbSet<TodoTask> Tasks { get; set; }
        public DbSet<Subtask> Subtasks { get; set; }

        #endregion

        #region Configuring

        /// <summary>
        /// Configuration.
        /// </summary>
        /// <param name="optionsBuilder"></param>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Use Sqlite. Place file in directory
            optionsBuilder.UseSqlite("Data Source=Tolldo.db");
        } 

        #endregion

        #region Model Creating

        /// <summary>
        /// Configures the database structure and relationships
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            /* Fluent API */
            // Set primary keys
            modelBuilder.Entity<Todo>()
                .HasKey(a => a.Id);
            modelBuilder.Entity<TodoTask>()
                .HasKey(a => a.Id);
            modelBuilder.Entity<Subtask>()
                .HasKey(a => a.Id);

            // Set required
            modelBuilder.Entity<TodoTask>()
                .Property(a => a.TodoId).IsRequired();
            modelBuilder.Entity<Subtask>()
                .Property(a => a.TodoTaskId).IsRequired();

            // Set behavior
            modelBuilder.Entity<Todo>()
                .HasMany(x => x.Tasks)
                .WithOne()
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<TodoTask>()
                .HasMany(x => x.Subtasks)
                .WithOne()
                .OnDelete(DeleteBehavior.Cascade);
        }

        #endregion
    }
}
