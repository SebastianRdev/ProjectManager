using System;

namespace ProjectManager.Application.DTOs
{
    public class TaskItemDto
    {
        public Guid Id { get; set; }
        public Guid ProjectId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Priority { get; set; } = string.Empty;
        public int Order { get; set; }
        public bool IsCompleted { get; set; }
    }

    public class CreateTaskItemDto
    {
        public string Title { get; set; } = string.Empty;
        public string Priority { get; set; } = "Medium";
        public int Order { get; set; }
    }

    public class UpdateTaskItemDto
    {
        public string Title { get; set; } = string.Empty;
        public string Priority { get; set; } = "Medium";
        public int Order { get; set; }
    }
}
