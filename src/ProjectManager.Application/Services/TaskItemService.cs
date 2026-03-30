using ProjectManager.Application.Interfaces;
using ProjectManager.Application.DTOs;
using ProjectManager.Domain.Entities;
using ProjectManager.Domain.Enums;

namespace ProjectManager.Application.Services
{
    public class TaskItemService : ITaskItemService
    {
        private readonly IGenericRepository<TaskItem> _repository;
        private readonly IGenericRepository<Project> _projectRepository;

        public TaskItemService(IGenericRepository<TaskItem> repository, IGenericRepository<Project> projectRepository)
        {
            _repository = repository;
            _projectRepository = projectRepository;
        }

        public async Task<IEnumerable<TaskItemDto>> GetByProjectIdAsync(Guid projectId)
        {
            var tasks = await _repository.FindAsync(t => t.ProjectId == projectId);
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

        public async Task<TaskItemDto> CreateAsync(Guid projectId, CreateTaskItemDto dto)
        {
            var project = await _projectRepository.GetByIdAsync(projectId);
            if (project == null) throw new KeyNotFoundException("Project not found");

            var allTasks = (await _repository.FindAsync(t => t.ProjectId == projectId)).ToList();
            var totalTasks = allTasks.Count;

            if (dto.Order < 1 || dto.Order > totalTasks + 1)
            {
                throw new InvalidOperationException($"El orden debe estar entre 1 y {totalTasks + 1}.");
            }

            var tasksToShift = allTasks.Where(t => t.Order >= dto.Order).ToList();
            foreach (var t in tasksToShift)
            {
                t.Order++;
                await _repository.UpdateAsync(t);
            }

            var priority = Enum.TryParse<TaskPriority>(dto.Priority, out var p) ? p : TaskPriority.Medium;

            var task = new TaskItem
            {
                Id = Guid.NewGuid(),
                ProjectId = projectId,
                Title = dto.Title,
                Priority = priority,
                Order = dto.Order,
                IsCompleted = false
            };

            await _repository.AddAsync(task);

            return new TaskItemDto
            {
                Id = task.Id,
                ProjectId = task.ProjectId,
                Title = task.Title,
                Priority = task.Priority.ToString(),
                Order = task.Order,
                IsCompleted = task.IsCompleted
            };
        }

        public async Task UpdateAsync(Guid id, UpdateTaskItemDto dto)
        {
            var task = await _repository.GetByIdAsync(id);
            if (task == null) throw new KeyNotFoundException("Task not found");

            var allTasks = (await _repository.FindAsync(t => t.ProjectId == task.ProjectId)).ToList();
            var totalTasks = allTasks.Count;

            if (dto.Order < 1 || dto.Order > totalTasks)
            {
                throw new InvalidOperationException($"El orden debe estar entre 1 y {totalTasks}.");
            }

            if (task.Order != dto.Order)
            {
                int oldOrder = task.Order;
                int newOrder = dto.Order;

                if (newOrder < oldOrder)
                {
                    var tasksToShift = allTasks.Where(t => t.Order >= newOrder && t.Order < oldOrder && t.Id != task.Id).ToList();
                    foreach (var t in tasksToShift)
                    {
                        t.Order++;
                        await _repository.UpdateAsync(t);
                    }
                }
                else if (newOrder > oldOrder)
                {
                    var tasksToShift = allTasks.Where(t => t.Order > oldOrder && t.Order <= newOrder && t.Id != task.Id).ToList();
                    foreach (var t in tasksToShift)
                    {
                        t.Order--;
                        await _repository.UpdateAsync(t);
                    }
                }
                
                task.Order = newOrder;
            }

            task.Title = dto.Title;
            task.Priority = Enum.TryParse<TaskPriority>(dto.Priority, out var p) ? p : TaskPriority.Medium;

            await _repository.UpdateAsync(task);
        }

        public async Task DeleteAsync(Guid id)
        {
            var task = await _repository.GetByIdAsync(id);
            if (task != null)
            {
                var allTasks = (await _repository.FindAsync(t => t.ProjectId == task.ProjectId)).ToList();
                var tasksToShift = allTasks.Where(t => t.Order > task.Order && t.Id != task.Id).ToList();
                foreach (var t in tasksToShift)
                {
                    t.Order--;
                    await _repository.UpdateAsync(t);
                }
                await _repository.DeleteAsync(id);
            }
        }

        public async Task CompleteAsync(Guid id)
        {
            var task = await _repository.GetByIdAsync(id);
            if (task == null) throw new KeyNotFoundException("Task not found");

            task.IsCompleted = true;
            await _repository.UpdateAsync(task);
        }

        public async Task ReorderAsync(Guid id, int newOrder)
        {
            var task = await _repository.GetByIdAsync(id);
            if (task == null) throw new KeyNotFoundException("Task not found");

            var allTasks = (await _repository.FindAsync(t => t.ProjectId == task.ProjectId)).ToList();
            var totalTasks = allTasks.Count;

            if (newOrder < 1 || newOrder > totalTasks)
            {
                throw new InvalidOperationException($"El orden debe estar entre 1 y {totalTasks}.");
            }

            if (task.Order != newOrder)
            {
                int oldOrder = task.Order;

                if (newOrder < oldOrder)
                {
                    var tasksToShift = allTasks.Where(t => t.Order >= newOrder && t.Order < oldOrder && t.Id != task.Id).ToList();
                    foreach (var t in tasksToShift)
                    {
                        t.Order++;
                        await _repository.UpdateAsync(t);
                    }
                }
                else if (newOrder > oldOrder)
                {
                    var tasksToShift = allTasks.Where(t => t.Order > oldOrder && t.Order <= newOrder && t.Id != task.Id).ToList();
                    foreach (var t in tasksToShift)
                    {
                        t.Order--;
                        await _repository.UpdateAsync(t);
                    }
                }
                
                task.Order = newOrder;
                await _repository.UpdateAsync(task);
            }
        }
    }
}
