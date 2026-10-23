// Import application data transfer objects
using TaskFlowPro.Application.DTOs;

// Import domain entity models
using TaskFlowPro.Domain.Entities;

namespace TaskFlowPro.Application.Interfaces
{
    // Define the interface contract specifying all user-related business operations
    public interface IUserService
    {
        // Contract method to create a new user record
        Task<User> CreateUserAsync(CreateUserDto dto);

        // Contract method to retrieve all user records
        Task<IEnumerable<User>> GetAllUsersAsync();

        // Contract method to retrieve a single user by their unique identifier
        Task<User?> GetUserByIdAsync(int id);
    }
}