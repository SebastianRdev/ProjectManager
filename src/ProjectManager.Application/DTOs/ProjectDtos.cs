using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ProjectManager.Application.DTOs
{
    public class ProjectDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public List<TaskItemDto> Tasks { get; set; } = new List<TaskItemDto>();
    }

    public class CreateProjectDto
    {
        [Required(ErrorMessage = "Project name is required.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Description cannot be empty.")]
        public string Description { get; set; } = string.Empty;
    }

    public class UpdateProjectDto
    {
        [Required(ErrorMessage = "Project name is required.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Description cannot be empty.")]
        public string Description { get; set; } = string.Empty;
    }

    public class ProjectSummaryDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public int TotalTasks { get; set; }
        public int CompletedTasks { get; set; }
    }
}
