// Import FluentValidation namespace for defining validation rules
using FluentValidation;

// Import application DTO namespace
using TaskFlowPro.Application.DTOs;

namespace TaskFlowPro.Application.Validators // Define namespace for application validators
{
    public class CreateUserDtoValidator : AbstractValidator<CreateUserDto> // Inherit from AbstractValidator targeting CreateUserDto
    {
        public CreateUserDtoValidator() // Constructor defining validation rule constraints
        {
            RuleFor(x => x.Username) // Target the Username property on CreateUserDto
                .NotEmpty().WithMessage("Username is required.") // Ensure username is not empty
                .MinimumLength(3).WithMessage("Username must be at least 3 characters long."); // Ensure minimum username length

            RuleFor(x => x.Email) // Target the Email property on CreateUserDto
                .NotEmpty().WithMessage("Email address is required.") // Ensure email is not empty
                .EmailAddress().WithMessage("A valid email address is required."); // Ensure email matches valid format pattern
        }
    }
}