using Microsoft.AspNetCore.Mvc;
using backend.Services;
using backend.DTOs;

namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;
        private readonly ILogger<EmployeeController> _logger;

        public EmployeeController(IEmployeeService employeeService, ILogger<EmployeeController> logger)
        {
            _employeeService = employeeService;
            _logger = logger;
        }

        /// <summary>
        /// Get list of employees for an employer
        /// </summary>
        /// <param name="employerId">ID of the employer</param>
        /// <param name="searchTerm">Search term for name or email</param>
        /// <param name="estadoFilter">Filter by employee status (Activa, Pendiente, Desactivada)</param>
        /// <param name="sortBy">Sort field (nombre, email, puesto, salario, estado, fecha)</param>
        /// <param name="sortDirection">Sort direction (asc, desc)</param>
        /// <returns>List of employees matching the criteria</returns>
        [HttpGet("list/{employerId}")]
        public async Task<IActionResult> GetEmployeeList(
            int employerId,
            [FromQuery] string? searchTerm = null,
            [FromQuery] string? estadoFilter = null,
            [FromQuery] string? sortBy = "nombre",
            [FromQuery] string? sortDirection = "asc")
        {
            try
            {
                // TODO: In a real application, validate that the current user is authorized to view this employer's employees
                if (employerId <= 0)
                {
                    return BadRequest(new { message = "ID de empleador inválido" });
                }

                var filter = new EmployeeFilterDto
                {
                    SearchTerm = searchTerm,
                    EstadoFilter = estadoFilter,
                    SortBy = sortBy,
                    SortDirection = sortDirection
                };

                var result = await _employeeService.GetEmployeeListAsync(employerId, filter);
                
                _logger.LogInformation("Employee list retrieved for employer {EmployerId}. Found {Count} employees", 
                    employerId, result.TotalCount);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving employee list for employer {EmployerId}", employerId);
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Get employee details by ID
        /// </summary>
        /// <param name="employerId">ID of the employer</param>
        /// <param name="employeeId">ID of the employee</param>
        /// <returns>Employee details</returns>
        [HttpGet("{employeeId}/employer/{employerId}")]
        public async Task<IActionResult> GetEmployeeById(int employerId, int employeeId)
        {
            try
            {
                if (employerId <= 0 || employeeId <= 0)
                {
                    return BadRequest(new { message = "ID de empleador o empleado inválido" });
                }

                var employee = await _employeeService.GetEmployeeByIdAsync(employeeId, employerId);
                
                if (employee == null)
                {
                    return NotFound(new { message = "Empleado no encontrado" });
                }

                var employeeDto = new EmployeeListDto
                {
                    Id = employee.Id,
                    NombreCompleto = employee.NombreCompleto,
                    Email = employee.Email,
                    Celular = employee.Celular,
                    Puesto = employee.Puesto,
                    Salario = employee.Salario,
                    Estado = employee.Estado.ToString(),
                    FechaContratacion = employee.FechaContratacion
                };

                return Ok(new { employee = employeeDto });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving employee {EmployeeId} for employer {EmployerId}", 
                    employeeId, employerId);
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Add sample employees for testing (Development/Demo only)
        /// </summary>
        /// <param name="employerId">ID of the employer</param>
        /// <returns>Success message</returns>
        [HttpPost("add-sample-data/{employerId}")]
        public async Task<IActionResult> AddSampleEmployees(int employerId)
        {
            try
            {
                if (employerId <= 0)
                {
                    return BadRequest(new { message = "ID de empleador inválido" });
                }

                var success = await _employeeService.AddSampleEmployeesAsync(employerId);
                
                if (success)
                {
                    return Ok(new { message = "Empleados de ejemplo agregados exitosamente" });
                }
                else
                {
                    return StatusCode(500, new { message = "Error al agregar empleados de ejemplo" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding sample employees for employer {EmployerId}", employerId);
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Get available employee status options
        /// </summary>
        /// <returns>List of employee status options</returns>
        [HttpGet("status-options")]
        public IActionResult GetStatusOptions()
        {
            var statusOptions = new[]
            {
                new { value = "Activa", label = "Activa" },
                new { value = "Pendiente", label = "Pendiente" },
                new { value = "Desactivada", label = "Desactivada" }
            };

            return Ok(new { statusOptions });
        }
    }
}
