namespace TaskFlowPro.Domain.Enums // Groups related enums into the Domain Enums namespace
{
    public enum Priority // Defines a custom enumerated data type for task urgency levels
    {
        Low,    // Enum value 0: Low priority work item
        Medium, // Enum value 1: Standard or default priority work item
        High    // Enum value 2: Urgent or critical priority work item
    }
}