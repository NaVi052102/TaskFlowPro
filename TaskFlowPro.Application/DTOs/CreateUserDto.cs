namespace TaskFlowPro.Application.DTOs
{
    // Data transfer object for handling incoming request payloads to create a new user
    public class CreateUserDto
    {
        // The username property for the user account
        public string Username { get; set; } = string.Empty;

        // The email address property for the user account
        public string Email { get; set; } = string.Empty;
    }
}