namespace TaskFlowPro.Application.DTOs
{
    public class VerifyEmailDto
    {
        public required string Email { get; set; } // Add 'required' modifier
        public required string Token { get; set; } // Add 'required' modifier
    }
}