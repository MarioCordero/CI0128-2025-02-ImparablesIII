using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using backend.DTOs;
using backend.Services;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ProjectController : ControllerBase
    {
        private readonly IProjectService _projectService;

        public ProjectController(IProjectService projectService)
        {
            _projectService = projectService;
        }

        [HttpPost]
        public async Task<ActionResult<ProjectResponseDto>> CreateProject([FromBody] CreateProjectDto createProjectDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Get employer ID from JWT token
                var employerIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (employerIdClaim == null || !int.TryParse(employerIdClaim.Value, out var employerId))
                {
                    return Unauthorized("ID de empleador no válido");
                }

                var project = await _projectService.CreateProjectAsync(createProjectDto, employerId);
                return CreatedAtAction(nameof(GetProject), new { id = project.Id }, project);
            }
            catch (ArgumentException ex)
            {
                return Conflict(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        [HttpGet]
        public async Task<ActionResult<List<CompanyListDto>>> GetEmployerCompanies()
        {
            try
            {
                // Get employer ID from JWT token
                var employerIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (employerIdClaim == null || !int.TryParse(employerIdClaim.Value, out var employerId))
                {
                    return Unauthorized("ID de empleador no válido");
                }

                var projects = await _projectService.GetProjectsByEmployerAsync(employerId);
                return Ok(projects);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CompanyResponseDto>> GetCompany(int id)
        {
            try
            {
                var project = await _projectService.GetProjectByIdAsync(id);

                if (project == null)
                {
                    return NotFound(new { message = "Proyecto no encontrado" });
                }

                return Ok(project);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }
    }
}