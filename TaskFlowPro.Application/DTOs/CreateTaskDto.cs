namespace TaskFlowPro.Application.DTOs
{
    // Data transfer object representing the payload required to create a new task
    public class CreateTaskDto
    {
        // The title or short summary of the task
        public string Title { get; set; } = string.Empty;

        // Detailed explanation of what needs to be done for the task
        public string Description { get; set; } = string.Empty;

        // Optional due date for the task
        public DateTime? DueDate { get; set; }

        // Foreign key linking the task to its parent project
        public int ProjectId { get; set; }

        // Optional foreign key linking the task to an assigned user
        public int? AssignedUserId { get; set; }
    }
}