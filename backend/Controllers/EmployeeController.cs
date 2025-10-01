using Microsoft.AspNetCore.Mvc;
using backend.Services;
using backend.DTOs;
using backend.Data;
using backend.Models;

namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;
        private readonly ILogger<EmployeeController> _logger;
        private readonly PlaniFyDbContext _context;

        public EmployeeController(IEmployeeService employeeService, ILogger<EmployeeController> logger, PlaniFyDbContext context)
        {
            _employeeService = employeeService;
            _logger = logger;
            _context = context;
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
        /// Insert a new employee into PlaniFy database (for testing)
        /// </summary>
        [HttpPost("planify/insert-sample")]
        public async Task<IActionResult> InsertSampleEmployee()
        {
            try
            {
                // Create a new person
                var persona = new Empleado
                {
                    Persona = new Persona
                    {
                        Correo = "maria.rodriguez@company.com",
                        Nombre = "Maria",
                        SegundoNombre = "Elena", 
                        Apellidos = "Rodriguez Vargas",
                        FechaNacimiento = new DateTime(1990, 5, 15),
                        Cedula = "118530245",
                        Rol = "Empleado",
                        Telefono = 88887777
                    },
                    Departamento = "Desarrollo",
                    TipoContrato = "Tiempo Completo",
                    TipoSalario = "Mensual",
                    Puesto = "Desarrolladora Senior",
                    FechaContratacion = DateTime.Now,
                    Salario = 1200000,
                    iban = "CR05015202001026284066"
                };

                _context.Empleados.Add(persona);
                await _context.SaveChangesAsync();

                return Ok(new { 
                    message = "Employee inserted successfully",
                    idPersona = persona.idPersona
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inserting sample employee");
                return StatusCode(500, new { message = "Error inserting employee: " + ex.Message });
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

        /// <summary>
        /// Get all employees from PlaniFy.Empleado table
        /// </summary>
        /// <returns>List of all empleados with their persona and empresa information</returns>
        [HttpGet("planify/all")]
        public async Task<IActionResult> GetAllEmpleados()
        {
            try
            {
                var empleados = await _employeeService.GetAllEmpleadosAsync();
                
                var result = empleados.Select(e => new
                {
                    idPersona = e.idPersona,
                    departamento = e.Departamento,
                    tipoContrato = e.TipoContrato,
                    tipoSalario = e.TipoSalario,
                    puesto = e.Puesto,
                    fechaContratacion = e.FechaContratacion,
                    salario = e.Salario,
                    iban = e.iban,
                    idEmpresa = e.idEmpresa,
                    persona = e.Persona != null ? new
                    {
                        id = e.Persona.Id,
                        correo = e.Persona.Correo,
                        nombre = e.Persona.Nombre,
                        segundoNombre = e.Persona.SegundoNombre,
                        apellidos = e.Persona.Apellidos,
                        nombreCompleto = e.Persona.NombreCompleto,
                        fechaNacimiento = e.Persona.FechaNacimiento,
                        cedula = e.Persona.Cedula,
                        telefono = e.Persona.Telefono
                    } : null,
                    empresa = e.Empresa != null ? new
                    {
                        id = e.Empresa.Id,
                        // Note: Empresa model doesn't have Nombre property yet
                        // nombre = e.Empresa.Nombre
                    } : null
                }).ToList();

                return Ok(new 
                { 
                    empleados = result,
                    totalCount = result.Count,
                    message = $"Se encontraron {result.Count} empleados en la base de datos"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving empleados from PlaniFy database");
                return StatusCode(500, new { message = "Error al obtener empleados de la base de datos" });
            }
        }

        /// <summary>
        /// Get employee by ID from PlaniFy.Empleado table
        /// </summary>
        /// <param name="idPersona">ID of the persona (empleado)</param>
        /// <returns>Empleado details with persona and empresa information</returns>
        [HttpGet("planify/{idPersona}")]
        public async Task<IActionResult> GetEmpleadoById(int idPersona)
        {
            try
            {
                if (idPersona <= 0)
                {
                    return BadRequest(new { message = "ID de persona inválido" });
                }

                var empleado = await _employeeService.GetEmpleadoByIdAsync(idPersona);
                
                if (empleado == null)
                {
                    return NotFound(new { message = "Empleado no encontrado" });
                }

                var result = new
                {
                    idPersona = empleado.idPersona,
                    departamento = empleado.Departamento,
                    tipoContrato = empleado.TipoContrato,
                    tipoSalario = empleado.TipoSalario,
                    puesto = empleado.Puesto,
                    fechaContratacion = empleado.FechaContratacion,
                    salario = empleado.Salario,
                    iban = empleado.iban,
                    idEmpresa = empleado.idEmpresa,
                    persona = empleado.Persona != null ? new
                    {
                        id = empleado.Persona.Id,
                        correo = empleado.Persona.Correo,
                        nombre = empleado.Persona.Nombre,
                        segundoNombre = empleado.Persona.SegundoNombre,
                        apellidos = empleado.Persona.Apellidos,
                        nombreCompleto = empleado.Persona.NombreCompleto,
                        fechaNacimiento = empleado.Persona.FechaNacimiento,
                        cedula = empleado.Persona.Cedula,
                        telefono = empleado.Persona.Telefono
                    } : null,
                    empresa = empleado.Empresa != null ? new
                    {
                        id = empleado.Empresa.Id,
                        // Note: Empresa model doesn't have Nombre property yet
                        // nombre = empleado.Empresa.Nombre
                    } : null
                };

                return Ok(new { empleado = result });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving empleado {IdPersona} from PlaniFy database", idPersona);
                return StatusCode(500, new { message = "Error al obtener empleado de la base de datos" });
            }
        }
    }
}
