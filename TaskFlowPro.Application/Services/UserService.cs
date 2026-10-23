// Import Entity Framework Core querying extensions
using Microsoft.EntityFrameworkCore;

// Import application DTOs and interfaces
using TaskFlowPro.Application.DTOs;
using TaskFlowPro.Application.Interfaces;

// Import domain entities
using TaskFlowPro.Domain.Entities;

namespace TaskFlowPro.Application.Services
{
    // Implement the IUserService interface containing core business logic for users
    public class UserService : IUserService
    {
        // Declare a private read-only field for the database context interface abstraction
        private readonly IApplicationDbContext _context;

        // Inject the database context abstraction via constructor dependency injection
        public UserService(IApplicationDbContext context)
        {
            _context = context;
        }

        // Business logic to create a new user record
        public async Task<User> CreateUserAsync(CreateUserDto dto)
        {
            // Instantiate a new User domain entity using data from the incoming DTO
            var user = new User
            {
                Username = dto.Username,
                Email = dto.Email
            };

            // Register the user entity with the database context
            _context.Users.Add(user);

            // Asynchronously save changes to the underlying database
            await _context.SaveChangesAsync();

            return user;
        }

        // Business logic to retrieve all user records
        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await _context.Users.ToListAsync();
        }

        // Business logic to retrieve a single user by unique identifier
        public async Task<User?> GetUserByIdAsync(int id)
        {
            return await _context.Users.FindAsync(id);
        }
    }
}