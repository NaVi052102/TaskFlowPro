using TaskFlowPro.Domain.Entities; // Import domain entity models namespace

namespace TaskFlowPro.Infrastructure.Persistence // Define namespace for infrastructure persistence components
{
    public static class DbInitializer // Define static class responsible for seeding initial database records
    {
        public static async Task InitializeAsync(AppDbContext context) // Asynchronous method to seed initial data if tables are empty
        {
            // Ensure the database and schema are created
            await context.Database.EnsureCreatedAsync();

            // Check if any users already exist in the database to prevent duplicate seeding
            if (context.Users.Any())
            {
                return; // Exit early if user data is already present
            }

            // Define initial user records array
            var users = new User[]
            {
                new User { Username = "alice_dev", Email = "alice@example.com" },
                new User { Username = "bob_engineer", Email = "bob@example.com" }
            };

            // Add users collection to the database context
            await context.Users.AddRangeAsync(users);

            // Asynchronously save user changes to the database storage
            await context.SaveChangesAsync();

            // Check if any projects already exist in the database
            if (context.Projects.Any())
            {
                return; // Exit early if project data is already present
            }

            // Define initial project records array linked to seeded users
            var projects = new Project[]
            {
                new Project 
                { 
                    Name = "TaskFlow Pro Architecture", 
                    Description = "Building a robust enterprise .NET 9 Clean Architecture backend", 
                    UserId = 1 
                },
                new Project 
                { 
                    Name = "Mobile Companion App", 
                    Description = "Developing Flutter cross-platform mobile interface", 
                    UserId = 2 
                }
            };

            // Add projects collection to the database context
            await context.Projects.AddRangeAsync(projects);

            // Asynchronously save project changes to the database storage
            await context.SaveChangesAsync();
        }
    }
}