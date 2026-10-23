using System.IdentityModel.Tokens.Jwt; // Import namespace for handling JWT tokens
using System.Security.Claims; // Import namespace for working with user claims
using System.Text; // Import namespace for handling text encoding
using Microsoft.Extensions.Configuration; // Import namespace for accessing configuration settings
using Microsoft.IdentityModel.Tokens; // Import namespace for cryptographic security tokens
using TaskFlowPro.Application.Interfaces; // Import application service interfaces namespace
using TaskFlowPro.Domain.Entities; // Import domain entities namespace

namespace TaskFlowPro.Infrastructure.Services // Define namespace for infrastructure services
{
    public class JwtTokenService : ITokenService // Implement ITokenService contract for JWT generation
    {
        private readonly IConfiguration _configuration; // Declare private read-only field for configuration access

        public JwtTokenService(IConfiguration configuration) // Constructor injecting configuration via dependency injection
        {
            _configuration = configuration; // Assign injected configuration instance to private field
        }

        public string GenerateToken(User user) // Business logic to generate a signed JWT bearer token for a user
        {
            var jwtSettings = _configuration.GetSection("JwtSettings"); // Retrieve JwtSettings configuration section
            var secretKey = jwtSettings["Secret"] ?? throw new InvalidOperationException("JWT Secret is not configured in appsettings.json."); // Ensure secret key is not null
            var keyBytes = Encoding.UTF8.GetBytes(secretKey); // Convert secret key string into UTF8 byte array

            var claims = new Claim[] // Define claims array representing user identity data embedded in token
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()), // Embed user ID claim
                new Claim(ClaimTypes.Name, user.Username), // Embed username claim
                new Claim(ClaimTypes.Email, user.Email) // Embed email claim
            };

            var credentials = new SigningCredentials(new SymmetricSecurityKey(keyBytes), SecurityAlgorithms.HmacSha256); // Create cryptographic signing credentials using HMAC-SHA256

            var tokenDescriptor = new SecurityTokenDescriptor // Configure token properties including claims, expiration, and signing
            {
                Subject = new ClaimsIdentity(claims), // Attach claims identity to token descriptor
                Expires = DateTime.UtcNow.AddHours(3), // Set token expiration time to 3 hours from current UTC time
                Issuer = jwtSettings["Issuer"], // Set token issuer string from configuration
                Audience = jwtSettings["Audience"], // Set token audience string from configuration
                SigningCredentials = credentials // Attach cryptographic signing credentials
            };

            var tokenHandler = new JwtSecurityTokenHandler(); // Instantiate JWT security token handler
            var token = tokenHandler.CreateToken(tokenDescriptor); // Create security token based on descriptor settings

            return tokenHandler.WriteToken(token); // Serialize and return security token as encrypted string
        }
    }
}