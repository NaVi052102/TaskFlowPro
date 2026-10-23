using TaskFlowPro.Domain.Entities; // Import domain entity models namespace

namespace TaskFlowPro.Application.Interfaces // Define namespace for application service contracts
{
    public interface ITokenService // Define interface contract for generating authentication tokens
    {
        string GenerateToken(User user); // Contract method that accepts a user and returns a signed JWT string
    }
}