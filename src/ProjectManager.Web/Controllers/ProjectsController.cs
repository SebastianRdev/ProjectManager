using ProjectManager.Application.Interfaces;
using ProjectManager.Application.DTOs;
using Microsoft.AspNetCore.Mvc;
using ProjectManager.Web.Filters;

namespace ProjectManager.Web.Controllers
{
    [SessionAuthorize]
    public class ProjectsController : Controller
    {
        private readonly IProjectService _projectService;

        public ProjectsController(IProjectService projectService)
        {
            _projectService = projectService;
        }

        public async Task<IActionResult> Index(string? status, int page = 1)
        {
            var projects = await _projectService.SearchAsync(page, 10, status);
            ViewBag.CurrentPage = page;
            ViewBag.StatusFilter = status ?? "";
            ViewBag.HasMore = projects.Count() == 10;
            return View(projects);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateProjectDto dto)
        {
            if (!ModelState.IsValid) return View(dto);

            await _projectService.CreateAsync(dto);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(Guid id)
        {
            var project = await _projectService.GetByIdAsync(id);
            if (project == null) return NotFound();

            var dto = new UpdateProjectDto { Name = project.Name, Description = project.Description };
            ViewBag.ProjectId = id;
            ViewBag.ProjectStatus = project.Status;
            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, UpdateProjectDto dto)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.ProjectId = id;
                return View(dto);
            }

            await _projectService.UpdateAsync(id, dto);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _projectService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Activate(Guid id)
        {
            try
            {
                await _projectService.ActivateAsync(id);
            }
            catch (InvalidOperationException ex)
            {
                TempData["Error"] = ex.Message;
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Complete(Guid id)
        {
            try
            {
                await _projectService.CompleteAsync(id);
            }
            catch (InvalidOperationException ex)
            {
                TempData["Error"] = ex.Message;
            }
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Summary(Guid id)
        {
            var summary = await _projectService.GetSummaryAsync(id);
            if (summary == null) return NotFound();
            return View(summary);
        }
    }
}
