using backend.DTOs;
using backend.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProjectController : ControllerBase
    {
        private readonly IProjectRepository _projectRepository;

        public ProjectController(IProjectRepository projectRepository)
        {
            _projectRepository = projectRepository;
        }

        [HttpGet]
        public async Task<ActionResult<List<ProjectResponseDto>>> GetAll()
        {
            try
            {
                var projects = await _projectRepository.GetAllAsync();
                return Ok(projects);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error retrieving projects", error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProjectResponseDto>> GetById(int id)
        {
            try
            {
                var project = await _projectRepository.GetByIdAsync(id);
                if (project == null)
                {
                    return NotFound(new { message = "Project not found" });
                }
                return Ok(project);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error retrieving project", error = ex.Message });
            }
        }

        [HttpGet("employer/{employerId}")]
        public async Task<ActionResult<List<ProjectResponseDto>>> GetByEmployerId(int employerId)
        {
            try
            {
                var projects = await _projectRepository.GetByEmployerIdAsync(employerId);
                return Ok(projects);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error retrieving projects", error = ex.Message });
            }
        }

        [HttpGet("dashboard/{employerId}")]
        public async Task<ActionResult<List<CompanyDashboardMainEmployerDto>>> GetProjectsForDashboard(int employerId)
        {
            try
            {
                var projects = await _projectRepository.GetProjectsForDashboardAsync(employerId);
                return Ok(projects);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error retrieving dashboard projects", error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<ActionResult<int>> Create([FromBody] CreateProjectDto projectDto)
        {
            try
            {
                // Validate if Legal ID already exists
                if (await _projectRepository.ExistsByLegalIdAsync(projectDto.LegalId))
                {
                    return BadRequest(new { message = "Legal ID already exists" });
                }

                // Validate if Email already exists
                if (await _projectRepository.ExistsByEmailAsync(projectDto.Email))
                {
                    return BadRequest(new { message = "Email already exists" });
                }

                // Crear el proyecto mapeando del DTO
                var project = new backend.Models.Project
                {
                    Nombre = projectDto.Nombre,
                    CedulaJuridica = projectDto.CedulaJuridica,
                    Email = projectDto.Email,
                    PeriodoPago = projectDto.PeriodoPago,
                    Telefono = projectDto.Telefono,
                    IdDireccion = await _projectRepository.CreateDireccionAsync(
                        projectDto.Provincia,
                        projectDto.Canton,
                        projectDto.Distrito,
                        projectDto.DireccionParticular)
                };

                var createdProject = await _projectRepository.CreateAsync(project);
                if (createdProject.Id > 0)
                {
                    return CreatedAtAction(nameof(GetById), new { id = createdProject.Id }, new { id = createdProject.Id });
                }

                return BadRequest(new { message = "Failed to create project" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error creating project", error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, [FromBody] UpdateProjectDto projectDto)
        {
            try
            {
                if (!await _projectRepository.ExistsByIdAsync(id))
                {
                    return NotFound(new { message = "Project not found" });
                }

                var success = await _projectRepository.UpdateAsync(id, projectDto);
                if (success)
                {
                    return NoContent();
                }

                return BadRequest(new { message = "Failed to update project" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error updating project", error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                if (!await _projectRepository.ExistsByIdAsync(id))
                {
                    return NotFound(new { message = "Project not found" });
                }

                var success = await _projectRepository.DeleteAsync(id);
                if (success)
                {
                    return NoContent();
                }

                return BadRequest(new { message = "Failed to delete project" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error deleting project", error = ex.Message });
            }
        }

        [HttpGet("{id}/employees/count")]
        public async Task<ActionResult<int>> GetActiveEmployeesCount(int id)
        {
            try
            {
                var count = await _projectRepository.CountActiveEmployeesAsync(id);
                return Ok(new { count });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error counting employees", error = ex.Message });
            }
        }

        [HttpGet("{id}/payroll")]
        public async Task<ActionResult<decimal>> GetMonthlyPayroll(int id)
        {
            try
            {
                var payroll = await _projectRepository.GetMonthlyPayrollAsync(id);
                return Ok(new { payroll });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error calculating payroll", error = ex.Message });
            }
        }

        [HttpPost("{id}/activate")]
        public async Task<ActionResult> Activate(int id)
        {
            try
            {
                var success = await _projectRepository.ActivateAsync(id);
                if (success)
                {
                    return Ok(new { message = "Project activated successfully" });
                }
                return BadRequest(new { message = "Failed to activate project" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error activating project", error = ex.Message });
            }
        }

        [HttpPost("{id}/deactivate")]
        public async Task<ActionResult> Deactivate(int id)
        {
            try
            {
                var success = await _projectRepository.DeactivateAsync(id);
                if (success)
                {
                    return Ok(new { message = "Project deactivated successfully" });
                }
                return BadRequest(new { message = "Failed to deactivate project" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error deactivating project", error = ex.Message });
            }
        }
    }
}