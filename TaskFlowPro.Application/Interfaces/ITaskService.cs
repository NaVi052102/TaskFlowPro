using TaskFlowPro.Application.DTOs; // Import application DTOs namespace
using TaskFlowPro.Domain.Entities; // Import domain entities namespace

namespace TaskFlowPro.Application.Interfaces // Define application interfaces namespace
{
    public interface ITaskService // Define task service contract interface
    {
        Task<IEnumerable<TaskItem>> GetTasksByUserIdAsync(int userId, int? projectId = null); // Retrieve tasks belonging to specific user with optional project filter
        Task<TaskItem?> GetTaskByIdAndUserIdAsync(int id, int userId); // Retrieve single task by ID and user ID
        Task<TaskItem> CreateTaskAsync(int userId, CreateTaskDto createTaskDto); // Create new task assigned to user
        Task<TaskItem?> CompleteTaskAsync(int id, int userId); // Mark task as completed or toggle completion status for user
        Task<bool> UpdateTaskAsync(int id, int userId, UpdateTaskDto updateTaskDto); // Update existing task details including completion and due date
        Task<bool> DeleteTaskAsync(int id, int userId); // Delete task matching user ownership
    }
}