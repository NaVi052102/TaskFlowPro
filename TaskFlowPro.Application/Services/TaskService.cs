using Microsoft.EntityFrameworkCore; // Import Entity Framework Core extension methods namespace
using TaskFlowPro.Application.DTOs; // Import application data transfer objects namespace
using TaskFlowPro.Application.Interfaces; // Import application interfaces namespace
using TaskFlowPro.Domain.Entities; // Import domain entities namespace

namespace TaskFlowPro.Application.Services // Define namespace for application services
{
    public class TaskService : ITaskService // Define TaskService class implementing ITaskService interface
    {
        private readonly IApplicationDbContext _context; // Declare private read-only database context field

        public TaskService(IApplicationDbContext context) // Constructor injecting database context via dependency injection
        {
            _context = context; // Assign injected database context instance to private field
        }

        public async Task<IEnumerable<TaskItem>> GetTasksByUserIdAsync(int userId, int? projectId = null) // Asynchronous method retrieving tasks for a user with optional project filtering
        {
            var query = _context.Tasks.Where(t => t.UserId == userId); // Initialize query filtering task records matching current user ID

            if (projectId.HasValue) // Check if an optional project identifier filter was provided
            {
                query = query.Where(t => t.ProjectId == projectId.Value); // Further filter query records matching the specified project ID
            }

            return await query.ToListAsync(); // Asynchronously execute filtered query and convert results to a list
        }

        public async Task<TaskItem?> GetTaskByIdAndUserIdAsync(int id, int userId) // Asynchronous method retrieving single task by identifier and user ID
        {
            return await _context.Tasks.FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId); // Query matching task owned by user
        }

        public async Task<TaskItem> CreateTaskAsync(int userId, CreateTaskDto createTaskDto) // Asynchronous method creating a new task for a user
        {
            var newTask = new TaskItem // Instantiate new TaskItem entity object
            {
                Title = createTaskDto.Title, // Assign task title from request payload
                Description = createTaskDto.Description, // Assign task description from request payload
                DueDate = createTaskDto.DueDate, // Assign due date property (can be null if not selected)
                ProjectId = createTaskDto.ProjectId, // Assign associated project identifier
                UserId = userId, // Assign authenticated user ID
                IsCompleted = false // Ensure newly created tasks start as uncompleted
            };

            _context.Tasks.Add(newTask); // Mark new task entity as added in Entity Framework change tracker
            await _context.SaveChangesAsync(default); // Asynchronously save changes to database storage

            return newTask; // Return created task entity instance
        }

        public async Task<TaskItem?> CompleteTaskAsync(int id, int userId) // Asynchronous method marking task as completed
        {
            var task = await _context.Tasks.FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId); // Find task matching ID and user ownership
            if (task == null) // Check if task record does not exist
            {
                return null; // Return null if task is not found or unauthorized
            }

            task.IsCompleted = !task.IsCompleted; // Toggle completion status flag (allows checking and unchecking)
            _context.Tasks.Update(task); // Mark task entity as modified in change tracker
            await _context.SaveChangesAsync(default); // Asynchronously save changes to database storage

            return task; // Return updated task entity instance
        }

        public async Task<bool> UpdateTaskAsync(int id, int userId, UpdateTaskDto updateTaskDto) // Asynchronous method updating an existing task
        {
            var task = await _context.Tasks.FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId); // Find task matching ID and user ownership
            if (task == null) // Check if task record does not exist
            {
                return false; // Return false if task is not found or unauthorized
            }

            task.Title = updateTaskDto.Title; // Update task title
            task.Description = updateTaskDto.Description; // Update task description
            task.DueDate = updateTaskDto.DueDate; // Update task due date
            task.IsCompleted = updateTaskDto.IsCompleted; // Update completion status correctly from request
            task.ProjectId = updateTaskDto.ProjectId; // Update associated project ID

            _context.Tasks.Update(task); // Mark task entity as modified in change tracker
            await _context.SaveChangesAsync(default); // Asynchronously save changes to database storage

            return true; // Return true confirming successful update
        }

        public async Task<bool> DeleteTaskAsync(int id, int userId) // Asynchronous method removing a task
        {
            var task = await _context.Tasks.FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId); // Find task matching ID and user ownership
            if (task == null) // Check if task record does not exist
            {
                return false; // Return false if task is not found or unauthorized
            }

            _context.Tasks.Remove(task); // Mark task entity as removed in change tracker
            await _context.SaveChangesAsync(default); // Asynchronously save deletion changes to database storage

            return true; // Return true confirming successful deletion
        }
    }
}