using backend.DTOs;
using backend.Services;
using backend.Constants;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using backend.Models;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProjectController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;
        private readonly ILogger<ProjectController> _logger;
        private readonly IProjectService _projectService;

        public ProjectController(
            IEmployeeService employeeService,
            ILogger<ProjectController> logger,
            IProjectService projectService)
        {
            _employeeService = employeeService;
            _projectService = projectService;
            _logger = logger;
        }

        // GET PROJECT BY ID
        [HttpGet("{id}")]
        public async Task<ActionResult<ProjectResponseDTO>> GetById(int id)
        {
            try
            {
                var project = await _projectService.GetProjectByIdAsync(id);
                if (project == null)
                {
                    return NotFound(new { message = ReturnMessagesConstants.Project.ProjectNotFound });
                }
                return Ok(project);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ReturnMessagesConstants.Project.ErrorRetrievingProject, error = ex.Message });
            }
        }

        // GET projects by employer ID
        [HttpGet("employer/{employerId}")]
        public async Task<ActionResult<List<ProjectResponseDTO>>> GetByEmployerId(int employerId)
        {
            try
            {
                var projects = await _projectService.GetProjectsByEmployerIdAsync(employerId);
                return Ok(projects);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ReturnMessagesConstants.Project.ErrorRetrievingProjects, error = ex.Message });
            }
        }

        // GET THE PROJECT'S DIRECTION BY DIRECTION ID
        [HttpGet("direction/{directionId}")]
        public async Task<ActionResult<DirectionDTO>> GetDirectionById(int directionId)
        {
            try
            {
                var direction = await _projectService.GetProjectDirectionByDirectionId(directionId);
                if (direction == null)
                {
                    return NotFound(new { message = "Dirección no encontrada" });
                }

                return Ok(direction);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al obtener la dirección", error = ex.Message });
            }
        }

        // GET PROJECTS FOR MAIN DASHBOARD BY EMPLOYER ID
        [HttpGet("dashboard/{employerId}")]
        public async Task<ActionResult<List<ProjectResponseDTO>>> GetProjectsForDashboard(int employerId)
        {
            try
            {
                var projects = await _projectService.GetProjectsForDashboardAsync(employerId);
                return Ok(projects);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al obtener proyectos para dashboard", error = ex.Message });
            }
        }

        // CREATE A NEW PROJECT REFRACT
        // [HttpPost]
        // public async Task<ActionResult<int>> Create([FromBody] CreateProjectDto projectDto)
        // {
        //     try
        //     {
        //         if (await _projectRepository.ExistsByLegalIdAsync(projectDto.CedulaJuridica.ToString()))
        //         {
        //             return BadRequest(new { message = ReturnMessagesConstants.Project.LegalIdAlreadyExists });
        //         }
        //         if (await _projectRepository.ExistsByEmailAsync(projectDto.Email))
        //         {
        //             return BadRequest(new { message = ReturnMessagesConstants.General.EmailAlreadyExists });
        //         }

        //         var project = new Project
        //         {
        //             Nombre = projectDto.Nombre,
        //             CedulaJuridica = projectDto.CedulaJuridica,
        //             EmployerId = projectDto.EmployerId,
        //             Email = projectDto.Email,
        //             PeriodoPago = projectDto.PeriodoPago,
        //             Telefono = projectDto.Telefono,
        //             IdDireccion = await _projectRepository.CreateDireccionAsync(
        //                 projectDto.Provincia,
        //                 projectDto.Canton,
        //                 projectDto.Distrito,
        //                 projectDto.DireccionParticular),
        //             MaximoBeneficios = projectDto.MaximoBeneficios
        //         };

        //         var createdProject = await _projectRepository.CreateAsync(project);
        //         return CreatedAtAction(nameof(GetById), new { id = createdProject.Id }, new { id = createdProject.Id });
        //     }
        //     catch (Exception ex)
        //     {
        //         return StatusCode(500, new { message = ReturnMessagesConstants.Project.ErrorCreatingProject, error = ex.Message });
        //     }
        // }

        // DELETE A PROJECT
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                var success = await _projectService.DeleteProjectAsync(id);
                if (success)
                {
                    return NoContent();
                }

                return BadRequest(new { message = ReturnMessagesConstants.Project.ErrorCreatingProject });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ReturnMessagesConstants.Project.ErrorDeletingProject, error = ex.Message });
            }
        }

        // GET THE COUNT OF ACTIVE EMPLOYEES IN A PROJECT
        [HttpGet("{id}/employees/count")]
        public async Task<ActionResult<int>> GetActiveEmployeesCount(int id)
        {
            try
            {
                var count = await _projectService.GetActiveEmployeesCountAsync(id);
                return Ok(new { count });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ReturnMessagesConstants.Project.ErrorCountingEmployees, error = ex.Message });
            }
        }

        // REFRACTO: GET THE MONTHLY PAYROLL OF A PROJECT
        // [HttpGet("{id}/payroll")]
        // public async Task<ActionResult<decimal>> GetMonthlyPayroll(int id)
        // {
        //     try
        //     {
        //         var payroll = await _projectRepository.GetMonthlyPayrollAsync(id);
        //         return Ok(new { payroll });
        //     }
        //     catch (Exception ex)
        //     {
        //         return StatusCode(500, new { message = ReturnMessagesConstants.Payroll.PayrollGeneratedSuccessfully, error = ex.Message });
        //     }
        // }

        // GET EMPLOYEES FOR A SPECIFIC PROJECT
        [HttpGet("{projectId:int}/employees")]
        public async Task<ActionResult<EmployeeListResponseDto>> GetProjectEmployees(int projectId)
        {
            try
            {
                _logger.LogInformation("Solicitud de empleados para el proyecto {ProjectId}", projectId);

                var result = await _employeeService.GetEmployeesByCompanyAsync(projectId);

                _logger.LogInformation("Enviados {Count} empleados para el proyecto {ProjectId}", result.TotalCount, projectId);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning("Argumento inválido para proyecto {ProjectId}: {Message}", projectId, ex.Message);
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error interno obteniendo empleados para el proyecto {ProjectId}", projectId);
                return StatusCode(500, new
                {
                    message = ReturnMessagesConstants.General.InternalServerError,
                    detail = ex.Message
                });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProject(int id, [FromBody] UpdateProjectDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _projectService.UpdateProjectAsync(id, dto);

            if (!result.Success)
                return NotFound(result.ErrorMessage);

            return Ok(result.Project);
        }

        // GET DASHBOARD METRICS FOR THE PROJECT (INDIVIDUALLY) DASHBOARD
        [HttpGet("{projectId}/dashboard/metrics")]
        public async Task<ActionResult<DashboardMetricsDTO>> GetDashboardMetrics(int projectId)
        {
            try
            {
                _logger.LogInformation("Obteniendo métricas de dashboard para el proyecto {ProjectId}", projectId);

                var metrics = await _projectService.GetDashboardMetricsAsync(projectId);

                if (metrics == null)
                {
                    return NotFound(new { message = "No se encontraron métricas para el proyecto especificado" });
                }

                _logger.LogInformation("Métricas obtenidas exitosamente para el proyecto {ProjectId}", projectId);
                return Ok(metrics);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning("Argumento inválido para proyecto {ProjectId}: {Message}", projectId, ex.Message);
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error interno obteniendo métricas para el proyecto {ProjectId}", projectId);
                return StatusCode(500, new
                {
                    message = ReturnMessagesConstants.General.InternalServerError,
                    detail = ex.Message
                });
            }
        }
    }
}