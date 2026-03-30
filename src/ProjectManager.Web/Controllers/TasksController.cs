using ProjectManager.Application.Interfaces;
using ProjectManager.Application.DTOs;
using Microsoft.AspNetCore.Mvc;
using ProjectManager.Web.Filters;

namespace ProjectManager.Web.Controllers
{
    [SessionAuthorize]
    public class TasksController : Controller
    {
        private readonly ITaskItemService _taskService;
        private readonly IProjectService _projectService;

        public TasksController(ITaskItemService taskService, IProjectService projectService)
        {
            _taskService = taskService;
            _projectService = projectService;
        }

        public async Task<IActionResult> Index(Guid projectId)
        {
            var project = await _projectService.GetByIdAsync(projectId);
            if (project == null) return NotFound();

            var tasks = await _taskService.GetByProjectIdAsync(projectId);
            ViewBag.Project = project;
            ViewBag.ProjectId = projectId;
            return View(tasks);
        }

        public IActionResult Create(Guid projectId)
        {
            ViewBag.ProjectId = projectId;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Guid projectId, CreateTaskItemDto dto)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.ProjectId = projectId;
                return View(dto);
            }

            try
            {
                await _taskService.CreateAsync(projectId, dto);
            }
            catch (InvalidOperationException ex)
            {
                TempData["Error"] = ex.Message;
                ViewBag.ProjectId = projectId;
                return View(dto);
            }

            return RedirectToAction(nameof(Index), new { projectId });
        }

        public async Task<IActionResult> Edit(Guid id, Guid projectId)
        {
            var tasks = await _taskService.GetByProjectIdAsync(projectId);
            var task = tasks.FirstOrDefault(t => t.Id == id);
            if (task == null) return NotFound();

            var dto = new UpdateTaskItemDto { Title = task.Title, Priority = task.Priority, Order = task.Order };
            ViewBag.TaskId = id;
            ViewBag.ProjectId = projectId;
            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, Guid projectId, UpdateTaskItemDto dto)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.TaskId = id;
                ViewBag.ProjectId = projectId;
                return View(dto);
            }

            try
            {
                await _taskService.UpdateAsync(id, dto);
            }
            catch (InvalidOperationException ex)
            {
                TempData["Error"] = ex.Message;
                ViewBag.TaskId = id;
                ViewBag.ProjectId = projectId;
                return View(dto);
            }

            return RedirectToAction(nameof(Index), new { projectId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id, Guid projectId)
        {
            await _taskService.DeleteAsync(id);
            return RedirectToAction(nameof(Index), new { projectId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Complete(Guid id, Guid projectId)
        {
            await _taskService.CompleteAsync(id);
            return RedirectToAction(nameof(Index), new { projectId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MoveUp(Guid id, Guid projectId, int currentOrder)
        {
            var tasks = (await _taskService.GetByProjectIdAsync(projectId)).ToList();
            var sorted = tasks.OrderBy(t => t.Order).ToList();
            var idx = sorted.FindIndex(t => t.Id == id);
            
            if (idx > 0)
            {
                try
                {
                    // If moving from index 1 (Order 2) up, we want it to be Order 1 (index).
                    await _taskService.ReorderAsync(id, idx);
                }
                catch (Exception ex)
                {
                    TempData["Error"] = ex.Message;
                }
            }
            return RedirectToAction(nameof(Index), new { projectId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MoveDown(Guid id, Guid projectId, int currentOrder)
        {
            var tasks = (await _taskService.GetByProjectIdAsync(projectId)).ToList();
            var sorted = tasks.OrderBy(t => t.Order).ToList();
            var idx = sorted.FindIndex(t => t.Id == id);
            
            if (idx < sorted.Count - 1)
            {
                try
                {
                    // If moving from index 2 (Order 3) down, we want it to be Order 4 (index + 2).
                    await _taskService.ReorderAsync(id, idx + 2);
                }
                catch (Exception ex)
                {
                    TempData["Error"] = ex.Message;
                }
            }
            return RedirectToAction(nameof(Index), new { projectId });
        }
    }
}
