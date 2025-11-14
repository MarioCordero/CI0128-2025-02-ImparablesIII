using backend.DTOs;
using backend.Repositories;
using backend.Services;
using backend.Constants;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Security.Claims;


namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProjectController : ControllerBase
    {
        private readonly IProjectRepository _projectRepository;
        private readonly IEmployeeService _employeeService;
        private readonly ILogger<ProjectController> _logger;
        private readonly IProjectService _projectService;


        public ProjectController(
            IProjectRepository projectRepository,
            IEmployeeService employeeService,
            ILogger<ProjectController> logger,
            IProjectService projectService)
        {
            _projectRepository = projectRepository;
            _employeeService = employeeService;
            _projectService = projectService;
            _logger = logger;
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
                return StatusCode(500, new { message = ReturnMessagesConstants.Project.ErrorRetrievingProjects, error = ex.Message });
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
                    return NotFound(new { message = ReturnMessagesConstants.Project.ProjectNotFound });
                }
                return Ok(project);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ReturnMessagesConstants.Project.ErrorRetrievingProject, error = ex.Message });
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
                return StatusCode(500, new { message = ReturnMessagesConstants.Project.ErrorRetrievingProjects, error = ex.Message });
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
                return StatusCode(500, new { message = ReturnMessagesConstants.Project.ErrorRetrievingProjects, error = ex.Message });
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
                    return BadRequest(new { message = ReturnMessagesConstants.Project.LegalIdAlreadyExists });
                }

                // Validate if Email already exists
                if (await _projectRepository.ExistsByEmailAsync(projectDto.Email))
                {
                    return BadRequest(new { message = ReturnMessagesConstants.General.EmailAlreadyExists });
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
                        projectDto.DireccionParticular),
                    MaximoBeneficios = projectDto.MaximoBeneficios
                };

                var createdProject = await _projectRepository.CreateAsync(project);
                if (createdProject.Id > 0)
                {
                    return CreatedAtAction(nameof(GetById), new { id = createdProject.Id }, new { id = createdProject.Id });
                }

                return BadRequest(new { message = ReturnMessagesConstants.Project.ErrorCreatingProject });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ReturnMessagesConstants.Project.ErrorCreatingProject, error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                if (!await _projectRepository.ExistsByIdAsync(id))
                {
                    return NotFound(new { message = ReturnMessagesConstants.Project.ProjectNotFound });
                }

                var success = await _projectRepository.DeleteAsync(id);
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
                return StatusCode(500, new { message = ReturnMessagesConstants.Project.ErrorCountingEmployees, error = ex.Message });
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
                return StatusCode(500, new { message = ReturnMessagesConstants.Payroll.PayrollGeneratedSuccessfully, error = ex.Message });
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
                    return Ok(new { message = ReturnMessagesConstants.Project.ProjectActivatedSuccessfully });
                }
                return BadRequest(new { message = ReturnMessagesConstants.Project.ErrorActivatingProject });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ReturnMessagesConstants.Project.ErrorActivatingProject, error = ex.Message });
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
                    return Ok(new { message = ReturnMessagesConstants.Project.ProjectDeactivatedSuccessfully });
                }
                return BadRequest(new { message = ReturnMessagesConstants.Project.ErrorDeactivatingProject });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ReturnMessagesConstants.Project.ErrorDeactivatingProject, error = ex.Message });
            }
        }

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

        [HttpGet("by-company/{companyId}")]
        public async Task<ActionResult<ProjectResponseDto>> GetByCompanyId(int companyId)
        {
            var project = await _projectRepository.GetProjectWithDireccionAsync(companyId);
            if (project == null)
                return NotFound();
            return Ok(project);
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
    }
}