namespace TaskFlowPro.Domain.Entities // Groups domain business entities together under the Domain namespace
{
    public class User // Represents a user account entity in the core domain
    {
        public int Id { get; set; } // Unique primary key identifier for the user record
        
        public string Username { get; set; } = string.Empty; // Username string initialized to empty to prevent null warnings
        
        public string Email { get; set; } = string.Empty; // Email address string initialized to empty to prevent null warnings
        
        public string PasswordHash { get; set; } = string.Empty; // Secure hashed password string initialized to empty
        
        // --- Email Verification Properties ---
        public bool IsEmailVerified { get; set; } = false; // Boolean flag tracking whether the user has verified their email via SMTP link
        
        public string? EmailVerificationToken { get; set; } = null; // Nullable string holding the unique token sent to the user's email
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // Timestamp recording when the user account was registered
        
        public ICollection<Project> Projects { get; set; } = new List<Project>(); // Navigation collection holding projects owned by this user
    }
}