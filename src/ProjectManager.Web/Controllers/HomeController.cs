using ProjectManager.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using ProjectManager.Web.Filters;

namespace ProjectManager.Web.Controllers
{
    [SessionAuthorize]
    public class HomeController : Controller
    {
        private readonly IProjectService _projectService;

        public HomeController(IProjectService projectService)
        {
            _projectService = projectService;
        }

        public async Task<IActionResult> Index()
        {
            var projects = await _projectService.SearchAsync(1, 100, null);
            ViewBag.TotalProjects = projects.Count();
            ViewBag.ActiveProjects = projects.Count(p => p.Status == "Active");
            ViewBag.CompletedProjects = projects.Count(p => p.Status == "Completed");
            ViewBag.TotalTasks = projects.Sum(p => p.TotalTasks);
            ViewBag.CompletedTasks = projects.Sum(p => p.CompletedTasks);
            return View();
        }
    }
}
