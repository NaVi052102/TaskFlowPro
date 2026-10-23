namespace TaskFlowPro.Application.DTOs // Define namespace for application data transfer objects
{
    public class LoginDto // Define public class representing incoming login request payload structure
    {
        public string Email { get; set; } = string.Empty; // Property holding user email address with default empty string initialization
        public string Password { get; set; } = string.Empty; // Property holding user password string with default empty string initialization
    }
}