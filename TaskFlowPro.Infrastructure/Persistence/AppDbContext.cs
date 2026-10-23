using Microsoft.EntityFrameworkCore; // Import Entity Framework Core namespace
using TaskFlowPro.Application.Interfaces; // Import application interfaces namespace
using TaskFlowPro.Domain.Entities; // Import domain entities namespace

namespace TaskFlowPro.Infrastructure.Persistence // Define persistence namespace for database context
{
    public class AppDbContext : DbContext, IApplicationDbContext // Define AppDbContext class inheriting DbContext and implementing IApplicationDbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) // Constructor accepting database context options via dependency injection
        {
        }

        public DbSet<TaskItem> Tasks { get; set; } = null!; // DbSet property implementing Tasks from IApplicationDbContext
        public DbSet<Project> Projects { get; set; } = null!; // DbSet property implementing Projects from IApplicationDbContext
        public DbSet<User> Users { get; set; } = null!; // DbSet property implementing Users from IApplicationDbContext

        protected override void OnModelCreating(ModelBuilder modelBuilder) // Override OnModelCreating to configure entity relationships and constraints
        {
            base.OnModelCreating(modelBuilder); // Invoke base class model configuration method
        }
    }
}