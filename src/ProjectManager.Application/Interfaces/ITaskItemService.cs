using ProjectManager.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProjectManager.Application.Interfaces
{
    public interface ITaskItemService
    {
        Task<IEnumerable<TaskItemDto>> GetByProjectIdAsync(Guid projectId);
        Task<TaskItemDto> CreateAsync(Guid projectId, CreateTaskItemDto dto);
        Task UpdateAsync(Guid id, UpdateTaskItemDto dto);
        Task DeleteAsync(Guid id);
        Task CompleteAsync(Guid id);
        Task ReorderAsync(Guid id, int newOrder);
    }
}
