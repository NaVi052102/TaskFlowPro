namespace TaskFlowPro.Application.DTOs // Groups our DTO classes together under the Application layer namespace
{
    public class ProjectDto // Defines the data structure returned to clients when fetching project details
    {
        public int Id { get; set; } // Unique integer identifier for the project record
        public string Name { get; set; } = string.Empty; // Title or name of the project
        public string Description { get; set; } = string.Empty; // Detailed explanation of what the project is about
        public DateTime CreatedAt { get; set; } // Timestamp recording exactly when the project was created
        public int UserId { get; set; } // Foreign key linking the project to its owner user
    }

    public class CreateProjectDto // Defines the data structure required from a client when creating a new project
    {
        public string Name { get; set; } = string.Empty; // Name provided by the user for the new project
        public string Description { get; set; } = string.Empty; // Description provided by the user for the new project
        public int UserId { get; set; } // ID of the user who is creating this project
    }
}