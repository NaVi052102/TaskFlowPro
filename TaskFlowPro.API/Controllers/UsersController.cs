using Microsoft.AspNetCore.Identity; // Import namespace for password hasher utility
using Microsoft.AspNetCore.Mvc; // Import MVC controller and action result namespaces
using Microsoft.EntityFrameworkCore; // Import Entity Framework Core extension methods namespace
using TaskFlowPro.Application.DTOs; // Import application data transfer objects namespace
using TaskFlowPro.Application.Interfaces; // Import application service interfaces namespace
using TaskFlowPro.Domain.Entities; // Import domain entities namespace
using TaskFlowPro.Infrastructure.Services; // Import infrastructure services namespace for email operations

namespace TaskFlowPro.API.Controllers // Define namespace for API controllers
{
    [Route("api/users")] // Define base URL routing prefix for user endpoints
    [ApiController] // Mark class as an API controller enabling automated model validation and binding behaviors
    public class UsersController : ControllerBase // Define UsersController class inheriting from ControllerBase base class
    {
        private readonly IApplicationDbContext _context; // Declare private read-only database context field for data operations
        private readonly ITokenService _tokenService; // Declare private read-only token service field for JWT generation
        private readonly IEmailService _emailService; // Declare private read-only email service field for sending verification emails
        private readonly PasswordHasher<User> _passwordHasher; // Declare private read-only password hasher utility instance

        public UsersController(
            IApplicationDbContext context, 
            ITokenService tokenService, 
            IEmailService emailService) // Constructor injecting database context, token service, and email service via dependency injection
        {
            _context = context; // Assign injected database context instance to private field
            _tokenService = tokenService; // Assign injected token service instance to private field
            _emailService = emailService; // Assign injected email service instance to private field
            _passwordHasher = new PasswordHasher<User>(); // Instantiate password hasher utility for secure credential handling
        }

        [HttpPost("register")] // Define HTTP POST endpoint mapping to api/users/register route
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto) // Asynchronous action method accepting registration payload from request body
        {
            // Check if email already exists in the database asynchronously
            var existingUser = await _context.Users // Access Users DbSet from database context
                .FirstOrDefaultAsync(u => u.Email == registerDto.Email); // Asynchronously search for a user record matching the provided email address

            if (existingUser != null) // Check if user record already exists
            {
                return BadRequest(new { message = "An account with this email already exists." }); // Return 400 Bad Request if email is taken
            }

            // Generate a secure unique verification token for email confirmation
            var verificationToken = Guid.NewGuid().ToString(); // Create unique string token

            // Create a new User domain entity instance
            var newUser = new User // Instantiate new User entity object
            {
                Username = registerDto.Username, // Assign username from request dto
                Email = registerDto.Email, // Assign email from request dto
                IsEmailVerified = false, // Initialize new accounts as unverified until they click the email link
                EmailVerificationToken = verificationToken // Assign generated token string
            };

            // Securely hash the user password before storage
            newUser.PasswordHash = _passwordHasher.HashPassword(newUser, registerDto.Password); // Generate secure cryptographic password hash

            _context.Users.Add(newUser); // Mark new user entity as added in Entity Framework change tracker
            await _context.SaveChangesAsync(default); // Asynchronously save changes to the database storage

            // Send real confirmation email using Gmail SMTP and our EmailService
            try
            {
                await _emailService.SendVerificationEmailAsync(newUser.Email, verificationToken); // Dispatch verification email
            }
            catch (Exception ex)
            {
                // Log the email failure if needed, but allow registration to complete or handle gracefully
                Console.WriteLine($"Warning: Failed to send confirmation email: {ex.Message}");
            }

            return Ok(new // Return 200 OK HTTP response containing success message
            {
                Message = "Registration successful! Please check your real Gmail inbox to verify your account.", // Success notification message property
                UserId = newUser.Id // Newly created user identifier property
            });
        }

        [HttpPost("verify-email")] // Define HTTP POST endpoint mapping to api/users/verify-email route
        public async Task<IActionResult> VerifyEmail([FromBody] VerifyEmailDto verifyDto) // Asynchronous action accepting email and token from request body
        {
            // Find the user in the database by their email address
            var user = await _context.Users // Access Users DbSet from database context
                .FirstOrDefaultAsync(u => u.Email == verifyDto.Email); // Asynchronously search for user record matching email

            if (user == null) // Check if user record was not found
            {
                return BadRequest(new { message = "User not found." }); // Return 400 Bad Request if user does not exist
            }

            if (user.IsEmailVerified) // Check if user's email is already verified
            {
                return BadRequest(new { message = "Email is already verified." }); // Return 400 Bad Request if already verified
            }

            if (user.EmailVerificationToken != verifyDto.Token) // Check if stored token matches the submitted token from the email link
            {
                return BadRequest(new { message = "Invalid or expired verification token." }); // Return 400 Bad Request if token mismatch
            }

            // Update user verification status upon successful validation
            user.IsEmailVerified = true; // Set email verification flag to true
            user.EmailVerificationToken = null; // Clear verification token so it cannot be reused

            await _context.SaveChangesAsync(default); // Asynchronously save updated user state changes to database storage

            return Ok(new { message = "Email verified successfully!" }); // Return 200 OK success response to frontend
        }

        [HttpPost("login")] // Define HTTP POST endpoint mapping to api/users/login route
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto) // Asynchronous action method accepting login payload from request body
        {
            // Find the user by email in the database asynchronously
            var user = await _context.Users // Access Users DbSet from database context
                .FirstOrDefaultAsync(u => u.Email == loginDto.Email); // Asynchronously search for a user record matching the provided email address

            if (user == null) // Check if user record was not found in database storage
            {
                return Unauthorized(new { message = "Invalid email or password." }); // Return 401 Unauthorized response if authentication fails
            }

            // Verify the submitted password against the stored cryptographic hash
            var verificationResult = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, loginDto.Password); // Check password validity

            if (verificationResult == PasswordVerificationResult.Failed) // Check if password verification failed
            {
                return Unauthorized(new { message = "Invalid email or password." }); // Return 401 Unauthorized response if password mismatch occurs
            }

            // Block unverified users from logging in until they confirm their email address
            if (!user.IsEmailVerified)
            {
                return Unauthorized(new { message = "Please verify your email address before logging in. Check your inbox for the confirmation link." }); // Return 401 if unverified
            }

            // Generate and return the signed JWT bearer token
            var token = _tokenService.GenerateToken(user); // Invoke token generation service passing authenticated user entity

            return Ok(new // Return 200 OK HTTP response containing success message and authentication token
            { 
                Message = "Login successful.", // Success notification message property
                Token = token // Generated JWT bearer token string property
            });
        }
    }
}