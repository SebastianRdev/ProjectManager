using System;
using System.Collections.Generic;
using ProjectManager.Domain.Enums;

namespace ProjectManager.Domain.Entities
{
    public class Project
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public ProjectStatus Status { get; set; } = ProjectStatus.Draft;

        public ICollection<TaskItem> Tasks { get; set; } = new List<TaskItem>();
    }
}
