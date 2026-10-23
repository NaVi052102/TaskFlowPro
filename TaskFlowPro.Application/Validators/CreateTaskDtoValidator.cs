// Import FluentValidation namespace for defining validation rules
using FluentValidation;

// Import application DTO namespace
using TaskFlowPro.Application.DTOs;

namespace TaskFlowPro.Application.Validators // Define namespace for application validators
{
    public class CreateTaskDtoValidator : AbstractValidator<CreateTaskDto> // Inherit from AbstractValidator targeting CreateTaskDto
    {
        public CreateTaskDtoValidator() // Constructor defining validation rule constraints
        {
            RuleFor(x => x.Title) // Target the Title property on CreateTaskDto
                .NotEmpty().WithMessage("Task title is required.") // Ensure title is not null or empty with custom error message string
                .MaximumLength(100).WithMessage("Task title must not exceed 100 characters."); // Ensure title length is within limit

            RuleFor(x => x.ProjectId) // Target the ProjectId property on CreateTaskDto
                .GreaterThan(0).WithMessage("A valid Project ID is required."); // Ensure project ID is a positive number

            RuleFor(x => x.AssignedUserId) // Target the AssignedUserId property on CreateTaskDto
                .GreaterThan(0).WithMessage("A valid Assigned User ID is required."); // Ensure assigned user ID is a positive number
        }
    }
}