using Microsoft.EntityFrameworkCore; // Import Entity Framework Core namespace
using TaskFlowPro.Domain.Entities; // Import domain entities namespace

namespace TaskFlowPro.Application.Interfaces // Define namespace for application interfaces
{
    public interface IApplicationDbContext // Define database context abstraction interface
    {
        DbSet<TaskItem> Tasks { get; } // Expose Tasks DbSet property
        DbSet<Project> Projects { get; } // Expose Projects DbSet property
        DbSet<User> Users { get; } // Expose Users DbSet property

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default); // Expose asynchronous save changes method with optional cancellation token
    }
}