using ProjectManager.Application.Interfaces;
using ProjectManager.Application.DTOs;
using Microsoft.AspNetCore.Mvc;
using ProjectManager.Web.Filters;

namespace ProjectManager.Web.Controllers
{
    [SessionAuthorize]
    public class ProjectTasksController : Controller
    {
        private readonly IProjectService _projectService;

        public ProjectTasksController(IProjectService projectService)
        {
            _projectService = projectService;
        }

        public async Task<IActionResult> Index()
        {
            // Fetch all projects
            var projects = await _projectService.SearchAsync(1, 100, null);
            
            // For each project, fetch its tasks
            var projectTasks = new List<ProjectTasksViewModel>();
            foreach (var project in projects)
            {
                var tasks = await _projectService.GetTasksByProjectIdAsync(project.Id);
                projectTasks.Add(new ProjectTasksViewModel
                {
                    Project = project,
                    Tasks = tasks.OrderBy(t => t.Order).ToList()
                });
            }

            return View(projectTasks);
        }
    }

    public class ProjectTasksViewModel
    {
        public ProjectSummaryDto Project { get; set; } = null!;
        public List<TaskItemDto> Tasks { get; set; } = new();
    }
}
