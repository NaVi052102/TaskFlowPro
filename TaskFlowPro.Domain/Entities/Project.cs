namespace TaskFlowPro.Domain.Entities // Groups domain business entities together under the Domain namespace
{
    public class Project // Represents a project entity in the core domain
    {
        public int Id { get; set; } // Unique primary key identifier for the project record

        public string Name { get; set; } = string.Empty; // Name of the project initialized to empty string to avoid null warnings

        public string Description { get; set; } = string.Empty; // Detailed description initialized to empty string

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // Timestamp of when the project was created

        public int UserId { get; set; } // Foreign key linking to the User who owns the project

        public User Owner { get; set; } = null!; // Navigation property to the owner User initialized with null-forgiving operator
        
    }
}