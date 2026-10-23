namespace TaskFlowPro.Domain.Enums // Groups related enums into the Domain Enums namespace
{
    public enum TaskStatus // Defines a custom enumerated data type accessible publicly across projects
    {
        ToDo,       // Enum value 0: Represents a task that has not been started yet
        InProgress, // Enum value 1: Represents a task currently being worked on
        Completed   // Enum value 2: Represents a finished task
    }
}