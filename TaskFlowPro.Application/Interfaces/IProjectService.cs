using TaskFlowPro.Application.DTOs; // Imports the DTO namespace so method signatures can use ProjectDto and CreateProjectDto

namespace TaskFlowPro.Application.Interfaces // Groups service contracts/interfaces together in Clean Architecture
{
    public interface IProjectService // Defines the contract (list of available methods) for project business logic
    {
        Task<IEnumerable<ProjectDto>> GetAllProjectsAsync(); // Contract method to retrieve a list of all projects asynchronously
        Task<ProjectDto?> GetProjectByIdAsync(int id); // Contract method to retrieve a single project by its unique ID asynchronously
        Task<ProjectDto> CreateProjectAsync(CreateProjectDto createProjectDto); // Contract method to insert a new project asynchronously
    }
}