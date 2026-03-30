using ProjectManager.Application.Interfaces;
using ProjectManager.Application.DTOs;
using ProjectManager.Domain.Entities;
using ProjectManager.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace ProjectManager.Application.Services
{
    public class ProjectService : IProjectService
    {
        private readonly IGenericRepository<Project> _repository;
        private readonly IGenericRepository<TaskItem> _taskRepository;

        public ProjectService(IGenericRepository<Project> repository, IGenericRepository<TaskItem> taskRepository)
        {
            _repository = repository;
            _taskRepository = taskRepository;
        }

        public async Task<IEnumerable<ProjectSummaryDto>> SearchAsync(int page, int pageSize, string? status)
        {
            var query = _repository.GetQueryable().Include(p => p.Tasks).AsQueryable();

            if (!string.IsNullOrEmpty(status) && Enum.TryParse<ProjectStatus>(status, out var statusEnum))
            {
                query = query.Where(p => p.Status == statusEnum);
            }

            var projects = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(p => new ProjectSummaryDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    Status = p.Status.ToString(),
                    TotalTasks = p.Tasks.Count,
                    CompletedTasks = p.Tasks.Count(t => t.IsCompleted)
                })
                .ToListAsync();

            return projects;
        }

        public async Task<ProjectDto?> GetByIdAsync(Guid id)
        {
            var project = await _repository.GetQueryable()
                .Include(p => p.Tasks)
                .FirstOrDefaultAsync(p => p.Id == id);
            if (project == null) return null;

            return new ProjectDto
            {
                Id = project.Id,
                Name = project.Name,
                Description = project.Description,
                Status = project.Status.ToString(),
                Tasks = project.Tasks?.OrderBy(t => t.Order).Select(t => new TaskItemDto
                {
                    Id = t.Id,
                    ProjectId = t.ProjectId,
                    Title = t.Title,
                    Priority = t.Priority.ToString(),
                    Order = t.Order,
                    IsCompleted = t.IsCompleted
                }).ToList() ?? new List<TaskItemDto>()
            };
        }

        public async Task<ProjectSummaryDto?> GetSummaryAsync(Guid id)
        {
            var project = await _repository.GetQueryable()
                .Include(p => p.Tasks)
                .FirstOrDefaultAsync(p => p.Id == id);
            if (project == null) return null;

            return new ProjectSummaryDto
            {
                Id = project.Id,
                Name = project.Name,
                Description = project.Description,
                Status = project.Status.ToString(),
                TotalTasks = project.Tasks.Count,
                CompletedTasks = project.Tasks.Count(t => t.IsCompleted)
            };
        }

        public async Task<IEnumerable<TaskItemDto>> GetTasksByProjectIdAsync(Guid projectId)
        {
            var tasks = await _taskRepository.FindAsync(t => t.ProjectId == projectId);
            return tasks.OrderBy(t => t.Order).Select(t => new TaskItemDto
            {
                Id = t.Id,
                ProjectId = t.ProjectId,
                Title = t.Title,
                Priority = t.Priority.ToString(),
                Order = t.Order,
                IsCompleted = t.IsCompleted
            });
        }

        public async Task<ProjectDto> CreateAsync(CreateProjectDto dto)
        {
            var project = new Project
            {
                Id = Guid.NewGuid(),
                Name = dto.Name,
                Description = dto.Description,
                Status = ProjectStatus.Draft
            };

            await _repository.AddAsync(project);

            return new ProjectDto
            {
                Id = project.Id,
                Name = project.Name,
                Description = project.Description,
                Status = project.Status.ToString(),
                Tasks = new List<TaskItemDto>()
            };
        }

        public async Task UpdateAsync(Guid id, UpdateProjectDto dto)
        {
            var project = await _repository.GetByIdAsync(id);
            if (project == null) throw new KeyNotFoundException("Project not found");

            project.Name = dto.Name;
            project.Description = dto.Description;

            await _repository.UpdateAsync(project);
        }

        public async Task DeleteAsync(Guid id)
        {
            await _repository.DeleteAsync(id);
        }

        public async Task ActivateAsync(Guid id)
        {
            var project = await _repository.GetQueryable()
                .Include(p => p.Tasks)
                .FirstOrDefaultAsync(p => p.Id == id);
            if (project == null) throw new KeyNotFoundException("Project not found");

            if (project.Tasks == null || !project.Tasks.Any())
            {
                throw new InvalidOperationException("Cannot activate a project with no tasks.");
            }

            project.Status = ProjectStatus.Active;
            await _repository.UpdateAsync(project);
        }

        public async Task CompleteAsync(Guid id)
        {
            var project = await _repository.GetQueryable()
                .Include(p => p.Tasks)
                .FirstOrDefaultAsync(p => p.Id == id);
            if (project == null) throw new KeyNotFoundException("Project not found");

            if (project.Tasks == null || !project.Tasks.Any())
            {
                throw new InvalidOperationException("Cannot complete a project with no tasks.");
            }

            if (project.Tasks.Any(t => !t.IsCompleted))
            {
                throw new InvalidOperationException("Cannot complete a project with pending tasks.");
            }

            project.Status = ProjectStatus.Completed;
            await _repository.UpdateAsync(project);
        }
    }
}
