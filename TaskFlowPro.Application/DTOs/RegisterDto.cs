namespace TaskFlowPro.Application.DTOs // Define namespace for application data transfer objects
{
    public class RegisterDto // Define public class representing incoming user registration payload structure
    {
        public string Username { get; set; } = string.Empty; // Property holding desired username with default empty string initialization
        public string Email { get; set; } = string.Empty; // Property holding user email address with default empty string initialization
        public string Password { get; set; } = string.Empty; // Property holding user password string with default empty string initialization
    }
}