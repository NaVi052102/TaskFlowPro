using Microsoft.EntityFrameworkCore; // Imports EF Core extension methods like ToListAsync for database queries
using TaskFlowPro.Application.DTOs; // Imports DTO models for mapping database entities to clean response objects
using TaskFlowPro.Application.Interfaces; // Imports interfaces like IApplicationDbContext and IProjectService
using TaskFlowPro.Domain.Entities; // Imports domain database entities like Project

namespace TaskFlowPro.Application.Services // Groups concrete service implementation classes together
{
    public class ProjectService : IProjectService // Implements the IProjectService contract containing our actual business logic
    {
        private readonly IApplicationDbContext _context; // Private readonly field holding our database abstraction interface

        public ProjectService(IApplicationDbContext context) // Constructor that injects IApplicationDbContext via Dependency Injection
        {
            _context = context; // Assigns the injected interface instance to our private field so methods can use it
        }

        public async Task<IEnumerable<ProjectDto>> GetAllProjectsAsync() // Method to fetch all project records asynchronously
        {
            return await _context.Projects // Accesses the Projects table DbSet through our interface contract
                .Select(p => new ProjectDto // Projects each database entity into a lightweight ProjectDto object
                {
                    Id = p.Id, // Maps the entity's database Id to the DTO Id
                    Name = p.Name, // Maps the entity's Name to the DTO Name
                    Description = p.Description, // Maps the entity's Description to the DTO Description
                    CreatedAt = p.CreatedAt, // Maps the entity's CreatedAt timestamp to the DTO CreatedAt
                    UserId = p.UserId // Maps the entity's owner UserId to the DTO UserId
                })
                .ToListAsync(); // Executes the SQL query against MySQL and returns the results as a List asynchronously
        }

        public async Task<ProjectDto?> GetProjectByIdAsync(int id) // Method to fetch a single project by its primary key ID asynchronously
        {
            var project = await _context.Projects.FindAsync(id); // Searches the Projects table for a record matching the primary key id
            if (project == null) return null; // Returns null immediately if no matching project record exists in the database

            return new ProjectDto // Manually maps the found database entity into a ProjectDto object
            {
                Id = project.Id, // Maps entity Id
                Name = project.Name, // Maps entity Name
                Description = project.Description, // Maps entity Description
                CreatedAt = project.CreatedAt, // Maps entity CreatedAt
                UserId = project.UserId // Maps entity UserId
            };
        }

        public async Task<ProjectDto> CreateProjectAsync(CreateProjectDto dto) // Method to create and save a new project record asynchronously
        {
            var project = new Project // Instantiates a new Domain Project entity using incoming request data
            {
                Name = dto.Name, // Assigns the name from the input DTO
                Description = dto.Description, // Assigns the description from the input DTO
                UserId = dto.UserId, // Assigns the owner user ID from the input DTO
                CreatedAt = DateTime.UtcNow // Automatically sets the creation timestamp to the current UTC time
            };

            _context.Projects.Add(project); // Queues the new project entity to be tracked and inserted by Entity Framework
            await _context.SaveChangesAsync(); // Commits the transaction and executes the INSERT statement into the MySQL database

            return new ProjectDto // Returns the newly created project data mapped back into a ProjectDto
            {
                Id = project.Id, // Maps the newly generated database ID
                Name = project.Name, // Maps the project name
                Description = project.Description, // Maps the project description
                CreatedAt = project.CreatedAt, // Maps the creation timestamp
                UserId = project.UserId // Maps the owner user ID
            };
        }
    }
}