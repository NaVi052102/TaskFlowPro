using System.Security.Claims; // Import namespace for working with user claims and identity
using Microsoft.AspNetCore.Authorization; // Import authorization attribute namespace
using Microsoft.AspNetCore.Mvc; // Import MVC controller and action result namespaces
using TaskFlowPro.Application.DTOs; // Import application data transfer objects namespace
using TaskFlowPro.Application.Interfaces; // Import application interfaces namespace

namespace TaskFlowPro.API.Controllers // Define namespace for API controllers
{
    [Authorize] // Lock down controller so only authenticated users with valid JWT tokens can access endpoints
    [Route("api/tasks")] // Define base URL routing prefix for task endpoints
    [ApiController] // Mark class as an API controller enabling automated model validation and binding behaviors
    public class TasksController : ControllerBase // Define TasksController class inheriting from ControllerBase base class
    {
        private readonly ITaskService _taskService; // Declare private read-only task service field for business logic operations

        public TasksController(ITaskService taskService) // Constructor injecting task service via dependency injection
        {
            _taskService = taskService; // Assign injected task service instance to private field
        }

        [HttpGet] // Define HTTP GET endpoint mapping to api/tasks route
        public async Task<IActionResult> GetAllTasks([FromQuery] int? projectId) // Asynchronous action method to retrieve tasks with optional query string project filter
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier); // Retrieve NameIdentifier claim from user principal

            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId)) // Validate that user ID claim exists and parses correctly as an integer
            {
                return Unauthorized(new { message = "Invalid token claims." }); // Return 401 Unauthorized if user ID cannot be determined
            }

            var tasks = await _taskService.GetTasksByUserIdAsync(userId, projectId); // Invoke task service method with user ID and project filter

            return Ok(tasks); // Return 200 OK HTTP response containing user and project-specific tasks list
        }

        [HttpGet("{id}")] // Define HTTP GET endpoint mapping to api/tasks/{id} route for retrieving a single task
        public async Task<IActionResult> GetTaskById(int id) // Asynchronous action method to retrieve a specific task by identifier
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier); // Retrieve user ID claim
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId)) // Validate user ID claim
            {
                return Unauthorized(new { message = "Invalid token claims." }); // Return 401 Unauthorized if invalid
            }

            var task = await _taskService.GetTaskByIdAndUserIdAsync(id, userId); // Fetch task by task ID and user ID
            if (task == null) // Check if task record is missing
            {
                return NotFound(new { message = "Task not found." }); // Return 404 Not Found if task doesn't exist
            }

            return Ok(task); // Return 200 OK with the retrieved task object
        }

        [HttpPost] // Define HTTP POST endpoint mapping to api/tasks route
        public async Task<IActionResult> CreateTask([FromBody] CreateTaskDto createTaskDto) // Asynchronous action method to create a new task
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier); // Retrieve NameIdentifier claim from user principal

            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId)) // Validate that user ID claim exists and parses correctly as an integer
            {
                return Unauthorized(new { message = "Invalid token claims." }); // Return 401 Unauthorized if user ID cannot be determined
            }

            var createdTask = await _taskService.CreateTaskAsync(userId, createTaskDto); // Invoke task service creation method including due date

            return Ok(new // Return 200 OK HTTP response confirming task creation success
            {
                Message = "Task created successfully.", // Success notification message property
                TaskId = createdTask.Id // Newly created task identifier property
            });
        }

        [HttpPut("{id}")] // Define HTTP PUT endpoint mapping to api/tasks/{id} route for updating tasks
        public async Task<IActionResult> UpdateTask(int id, [FromBody] UpdateTaskDto updateTaskDto) // Asynchronous action method to update an existing task
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier); // Retrieve user ID claim
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId)) // Validate user ID claim
            {
                return Unauthorized(new { message = "Invalid token claims." }); // Return 401 Unauthorized if invalid
            }

            var updated = await _taskService.UpdateTaskAsync(id, userId, updateTaskDto); // Invoke update task service method handling completion and due dates
            if (!updated) // Check if update operation failed or task wasn't found
            {
                return NotFound(new { message = "Task not found or unauthorized." }); // Return 404 Not Found if task doesn't exist
            }

            return Ok(new { message = "Task updated successfully." }); // Return 200 OK confirming successful update
        }

        [HttpPatch("{id}/complete")] // Define HTTP PATCH endpoint specifically for marking tasks as completed
        public async Task<IActionResult> CompleteTask(int id) // Asynchronous action method toggling task completion state
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier); // Retrieve user ID claim
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId)) // Validate user ID claim
            {
                return Unauthorized(new { message = "Invalid token claims." }); // Return 401 Unauthorized if invalid
            }

            var completedTask = await _taskService.CompleteTaskAsync(id, userId); // Invoke completion toggle method in task service
            if (completedTask == null) // Check if task record was not found
            {
                return NotFound(new { message = "Task not found." }); // Return 404 Not Found if missing
            }

            return Ok(new { message = "Task status updated successfully.", task = completedTask }); // Return updated task object back to frontend
        }

        [HttpDelete("{id}")] // Define HTTP DELETE endpoint mapping to api/tasks/{id} route for deleting tasks
        public async Task<IActionResult> DeleteTask(int id) // Asynchronous action method to delete a task
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier); // Retrieve user ID claim
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId)) // Validate user ID claim
            {
                return Unauthorized(new { message = "Invalid token claims." }); // Return 401 Unauthorized if invalid
            }

            var deleted = await _taskService.DeleteTaskAsync(id, userId); // Invoke delete task service method
            if (!deleted) // Check if deletion operation failed or task wasn't found
            {
                return NotFound(new { message = "Task not found or unauthorized." }); // Return 404 Not Found if task doesn't exist
            }

            return Ok(new { message = "Task deleted successfully." }); // Return 200 OK confirming successful deletion
        }
    }
}