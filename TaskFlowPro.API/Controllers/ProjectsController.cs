using System.Security.Claims; // Import namespace for working with user claims and identity
using Microsoft.AspNetCore.Authorization; // Import authorization attribute namespace
using Microsoft.AspNetCore.Mvc; // Import MVC controller and action result namespaces
using Microsoft.EntityFrameworkCore; // Import Entity Framework Core extension methods namespace
using TaskFlowPro.Application.DTOs; // Import application data transfer objects namespace
using TaskFlowPro.Application.Interfaces; // Import application interfaces namespace for IApplicationDbContext
using TaskFlowPro.Domain.Entities; // Import domain entities namespace

namespace TaskFlowPro.API.Controllers // Define namespace for API controllers
{
    [Authorize] // Lock down controller so only authenticated users with valid JWT tokens can access endpoints
    [Route("api/projects")] // Define base URL routing prefix for project endpoints
    [ApiController] // Mark class as an API controller enabling automated model validation and binding behaviors
    public class ProjectsController : ControllerBase // Define ProjectsController class inheriting from ControllerBase base class
    {
        private readonly IApplicationDbContext _context; // Declare private read-only database context field for data operations

        public ProjectsController(IApplicationDbContext context) // Constructor injecting database context via dependency injection
        {
            _context = context; // Assign injected database context instance to private field
        }

        [HttpGet] // Define HTTP GET endpoint mapping to api/projects route
        public async Task<IActionResult> GetAllProjects() // Asynchronous action method to retrieve projects belonging to the logged-in user
        {
            // Extract user ID claim string from the current JWT bearer token
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier); // Retrieve NameIdentifier claim from user principal

            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId)) // Validate that user ID claim exists and parses correctly as an integer
            {
                return Unauthorized(new { message = "Invalid token claims." }); // Return 401 Unauthorized if user ID cannot be determined
            }

            // Retrieve only the projects belonging to this specific authenticated user from database storage
            var projects = await _context.Projects // Access Projects DbSet from database context
                .Where(p => p.UserId == userId) // Filter project records matching current user ID
                .ToListAsync(); // Asynchronously execute query and convert results to a list

            return Ok(projects); // Return 200 OK HTTP response containing user-specific projects list
        }

        [HttpPost] // Define HTTP POST endpoint mapping to api/projects route
        public async Task<IActionResult> CreateProject([FromBody] CreateProjectDto createProjectDto) // Asynchronous action method to create a new project
        {
            // Extract user ID claim string from the current JWT bearer token
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier); // Retrieve NameIdentifier claim from user principal

            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId)) // Validate that user ID claim exists and parses correctly as an integer
            {
                return Unauthorized(new { message = "Invalid token claims." }); // Return 401 Unauthorized if user ID cannot be determined
            }

            // Create new Project domain entity instance associated with the logged-in user
            var newProject = new Project // Instantiate new Project entity object
            {
                Name = createProjectDto.Name, // Assign project name from request payload
                Description = createProjectDto.Description, // Assign project description from request payload
                UserId = userId // Automatically assign authenticated user ID from token claims
            };

            _context.Projects.Add(newProject); // Mark new project entity as added in Entity Framework change tracker
            await _context.SaveChangesAsync(default); // Asynchronously save changes to database storage

            return Ok(new // Return 200 OK HTTP response confirming project creation success
            {
                Message = "Project created successfully.", // Success notification message property
                ProjectId = newProject.Id // Newly created project identifier property
            });
        }

        [HttpPut("{id}")] // Define HTTP PUT endpoint mapping to api/projects/{id} route for updating an existing project
        public async Task<IActionResult> UpdateProject(int id, [FromBody] CreateProjectDto updateProjectDto) // Asynchronous action method accepting project identifier and update payload
        {
            // Extract user ID claim string from the current JWT bearer token
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier); // Retrieve NameIdentifier claim from user principal

            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId)) // Validate that user ID claim exists and parses correctly as an integer
            {
                return Unauthorized(new { message = "Invalid token claims." }); // Return 401 Unauthorized if user ID cannot be determined
            }

            // Find the project matching the ID and ensure it belongs to the authenticated user
            var project = await _context.Projects // Access Projects DbSet from database context
                .FirstOrDefaultAsync(p => p.Id == id && p.UserId == userId); // Query project by ID filtered by current user ownership

            if (project == null) // Check if project record was not found or belongs to another user
            {
                return NotFound(new { message = "Project not found or unauthorized." }); // Return 404 Not Found if project is inaccessible
            }

            // Update project entity properties with values from request payload
            project.Name = updateProjectDto.Name; // Update project name property
            project.Description = updateProjectDto.Description; // Update project description property

            _context.Projects.Update(project); // Mark project entity as modified in Entity Framework change tracker
            await _context.SaveChangesAsync(default); // Asynchronously save updates to database storage

            return Ok(new // Return 200 OK HTTP response confirming update success
            {
                Message = "Project updated successfully." // Success notification message property
            });
        }

        [HttpDelete("{id}")] // Define HTTP DELETE endpoint mapping to api/projects/{id} route for removing a project
        public async Task<IActionResult> DeleteProject(int id) // Asynchronous action method accepting project identifier for deletion
        {
            // Extract user ID claim string from the current JWT bearer token
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier); // Retrieve NameIdentifier claim from user principal

            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId)) // Validate that user ID claim exists and parses correctly as an integer
            {
                return Unauthorized(new { message = "Invalid token claims." }); // Return 401 Unauthorized if user ID cannot be determined
            }

            // Find the project matching the ID and ensure it belongs to the authenticated user
            var project = await _context.Projects // Access Projects DbSet from database context
                .FirstOrDefaultAsync(p => p.Id == id && p.UserId == userId); // Query project by ID filtered by current user ownership

            if (project == null) // Check if project record was not found or belongs to another user
            {
                return NotFound(new { message = "Project not found or unauthorized." }); // Return 404 Not Found if project is inaccessible
            }

            _context.Projects.Remove(project); // Mark project entity as removed in Entity Framework change tracker
            await _context.SaveChangesAsync(default); // Asynchronously save deletion changes to database storage

            return Ok(new // Return 200 OK HTTP response confirming deletion success
            {
                Message = "Project deleted successfully." // Success notification message property
            });
        }
    }
}