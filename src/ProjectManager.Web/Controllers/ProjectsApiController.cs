using ProjectManager.Application.Interfaces;
using ProjectManager.Application.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ProjectManager.Web.Controllers.Api
{
    [Route("api/projects")]
    [ApiController]
    [Authorize]
    public class ProjectsApiController : ControllerBase
    {
        private readonly IProjectService _projectService;

        public ProjectsApiController(IProjectService projectService)
        {
            _projectService = projectService;
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string? status = null, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var projects = await _projectService.SearchAsync(page, pageSize, status);
            return Ok(projects);
        }

        [HttpGet("{id}/tasks")]
        public async Task<IActionResult> GetTasks(Guid id)
        {
            var tasks = await _projectService.GetTasksByProjectIdAsync(id);
            return Ok(tasks);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateProjectDto dto)
        {
            var project = await _projectService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetSummary), new { id = project.Id }, project);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateProjectDto dto)
        {
            await _projectService.UpdateAsync(id, dto);
            return NoContent();
        }

        [HttpGet("{id}/summary")]
        public async Task<IActionResult> GetSummary(Guid id)
        {
            var summary = await _projectService.GetSummaryAsync(id);
            if (summary == null) return NotFound();
            return Ok(summary);
        }

        [HttpPatch("{id}/activate")]
        public async Task<IActionResult> Activate(Guid id)
        {
            try
            {
                await _projectService.ActivateAsync(id);
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpPatch("{id}/complete")]
        public async Task<IActionResult> Complete(Guid id)
        {
            try
            {
                await _projectService.CompleteAsync(id);
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }
    }
}
