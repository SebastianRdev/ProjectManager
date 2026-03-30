using ProjectManager.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProjectManager.Application.Interfaces
{
    public interface IProjectService
    {
        Task<IEnumerable<ProjectSummaryDto>> SearchAsync(int page, int pageSize, string? status);
        Task<ProjectDto?> GetByIdAsync(Guid id);
        Task<ProjectSummaryDto?> GetSummaryAsync(Guid id);
        Task<IEnumerable<TaskItemDto>> GetTasksByProjectIdAsync(Guid projectId);
        Task<ProjectDto> CreateAsync(CreateProjectDto dto);
        Task UpdateAsync(Guid id, UpdateProjectDto dto);
        Task DeleteAsync(Guid id);
        Task ActivateAsync(Guid id);
        Task CompleteAsync(Guid id);
    }
}
