using System;
using ProjectManager.Domain.Enums;

namespace ProjectManager.Domain.Entities
{
    public class TaskItem
    {
        public Guid Id { get; set; }
        public Guid ProjectId { get; set; }
        public string Title { get; set; } = string.Empty;
        public TaskPriority Priority { get; set; } = TaskPriority.Medium;
        public int Order { get; set; }
        public bool IsCompleted { get; set; }

        public Project? Project { get; set; }
    }
}
