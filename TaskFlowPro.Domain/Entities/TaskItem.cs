namespace TaskFlowPro.Domain.Entities // Define domain entities namespace
{
    public class TaskItem // Define TaskItem domain entity class
    {
        public int Id { get; set; } // Unique primary key identifier for the task
        public string Title { get; set; } = string.Empty; // Task title property initialized to empty string
        public string Description { get; set; } = string.Empty; // Task description property initialized to empty string
        public DateTime? DueDate { get; set; } // Optional due date property for task scheduling
        public bool IsCompleted { get; set; } // Flag indicating whether the task is completed

        public int ProjectId { get; set; } // Foreign key identifier for the parent project
        public Project? Project { get; set; } // Navigation property pointing to the parent project entity

        public int UserId { get; set; } // Foreign key identifier for the owning user
        public User? User { get; set; } // Navigation property pointing to the owning user entity
        
    }
}