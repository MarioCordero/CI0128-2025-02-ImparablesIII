using backend.Models;
using backend.DTOs;
using backend.Data;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace backend.Services
{
    public interface IEmployeeService
    {
        Task<EmployeeListResponseDto> GetEmployeeListAsync(int employerId, EmployeeFilterDto filter);
        Task<Employee?> GetEmployeeByIdAsync(int employeeId, int employerId);
        Task<bool> AddSampleEmployeesAsync(int employerId);
        Task<List<Empleado>> GetAllEmpleadosAsync();
        Task<Empleado?> GetEmpleadoByIdAsync(int idPersona);
    }

    public class EmployeeService : IEmployeeService
    {
        private readonly List<Employee> _employees; // In-memory storage for demo
        private readonly ILogger<EmployeeService> _logger;
        private readonly PlaniFyDbContext _context;

        public EmployeeService(ILogger<EmployeeService> logger, PlaniFyDbContext context)
        {
            _logger = logger;
            _context = context;
            _employees = new List<Employee>();
            
            // Add some sample data for testing
            SeedSampleData();
        }

        public async Task<EmployeeListResponseDto> GetEmployeeListAsync(int employerId, EmployeeFilterDto filter)
        {
            try
            {
                var query = _employees.Where(e => e.EmployerId == employerId).AsQueryable();

                // Apply search filter
                if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
                {
                    var searchTerm = filter.SearchTerm.ToLower();
                    query = query.Where(e => 
                        e.NombreCompleto.ToLower().Contains(searchTerm) ||
                        e.Email.ToLower().Contains(searchTerm));
                }

                // Apply status filter
                if (!string.IsNullOrWhiteSpace(filter.EstadoFilter))
                {
                    if (Enum.TryParse<EmployeeStatus>(filter.EstadoFilter, true, out var status))
                    {
                        query = query.Where(e => e.Estado == status);
                    }
                }

                // Apply sorting
                query = ApplySorting(query, filter.SortBy, filter.SortDirection);

                var employees = query.ToList();
                var totalCount = employees.Count;

                var result = new EmployeeListResponseDto
                {
                    Employees = employees.Select(e => new EmployeeListDto
                    {
                        Id = e.Id,
                        NombreCompleto = e.NombreCompleto,
                        Email = e.Email,
                        Celular = e.Celular,
                        Puesto = e.Puesto,
                        Salario = e.Salario,
                        Estado = GetStatusDisplayName(e.Estado),
                        FechaContratacion = e.FechaContratacion
                    }).ToList(),
                    TotalCount = totalCount,
                    Message = totalCount == 0 ? "No hay empleados registrados" : $"Se encontraron {totalCount} empleados"
                };

                _logger.LogInformation("Retrieved {Count} employees for employer {EmployerId}", totalCount, employerId);
                return await Task.FromResult(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving employees for employer {EmployerId}", employerId);
                return new EmployeeListResponseDto
                {
                    Message = "Error al obtener la lista de empleados"
                };
            }
        }

        public async Task<Employee?> GetEmployeeByIdAsync(int employeeId, int employerId)
        {
            return await Task.FromResult(
                _employees.FirstOrDefault(e => e.Id == employeeId && e.EmployerId == employerId));
        }

        public async Task<bool> AddSampleEmployeesAsync(int employerId)
        {
            try
            {
                // Remove existing sample data for this employer
                _employees.RemoveAll(e => e.EmployerId == employerId);

                var sampleEmployees = new List<Employee>
                {
                    new Employee
                    {
                        Id = GenerateId(),
                        Nombre = "María",
                        PrimerApellido = "González",
                        SegundoApellido = "López",
                        Email = "maria.gonzalez@company.com",
                        Celular = "8888-1234",
                        Puesto = "Desarrolladora Senior",
                        Salario = 850000,
                        Estado = EmployeeStatus.Activa,
                        EmployerId = employerId,
                        FechaContratacion = DateTime.Now.AddMonths(-6)
                    },
                    new Employee
                    {
                        Id = GenerateId(),
                        Nombre = "Carlos",
                        PrimerApellido = "Ramírez",
                        Email = "carlos.ramirez@company.com",
                        Celular = "8888-5678",
                        Puesto = "Analista de Datos",
                        Salario = 650000,
                        Estado = EmployeeStatus.Activa,
                        EmployerId = employerId,
                        FechaContratacion = DateTime.Now.AddMonths(-3)
                    },
                    new Employee
                    {
                        Id = GenerateId(),
                        Nombre = "Ana",
                        PrimerApellido = "Jiménez",
                        SegundoApellido = "Mora",
                        Email = "ana.jimenez@company.com",
                        Celular = "8888-9012",
                        Puesto = "Diseñadora UX/UI",
                        Salario = 700000,
                        Estado = EmployeeStatus.Pendiente,
                        EmployerId = employerId,
                        FechaContratacion = DateTime.Now.AddDays(-15)
                    },
                    new Employee
                    {
                        Id = GenerateId(),
                        Nombre = "Roberto",
                        PrimerApellido = "Vargas",
                        Email = "roberto.vargas@company.com",
                        Celular = "8888-3456",
                        Puesto = "Gerente de Proyecto",
                        Salario = 950000,
                        Estado = EmployeeStatus.Desactivada,
                        EmployerId = employerId,
                        FechaContratacion = DateTime.Now.AddYears(-1)
                    }
                };

                _employees.AddRange(sampleEmployees);
                _logger.LogInformation("Added {Count} sample employees for employer {EmployerId}", 
                    sampleEmployees.Count, employerId);

                return await Task.FromResult(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding sample employees for employer {EmployerId}", employerId);
                return false;
            }
        }

        private IQueryable<Employee> ApplySorting(IQueryable<Employee> query, string? sortBy, string? sortDirection)
        {
            var isDescending = string.Equals(sortDirection, "desc", StringComparison.OrdinalIgnoreCase);
            
            return sortBy?.ToLower() switch
            {
                "email" => isDescending ? query.OrderByDescending(e => e.Email) : query.OrderBy(e => e.Email),
                "puesto" => isDescending ? query.OrderByDescending(e => e.Puesto) : query.OrderBy(e => e.Puesto),
                "salario" => isDescending ? query.OrderByDescending(e => e.Salario) : query.OrderBy(e => e.Salario),
                "estado" => isDescending ? query.OrderByDescending(e => e.Estado) : query.OrderBy(e => e.Estado),
                "fecha" => isDescending ? query.OrderByDescending(e => e.FechaContratacion) : query.OrderBy(e => e.FechaContratacion),
                _ => query.OrderBy(e => e.Nombre).ThenBy(e => e.PrimerApellido) // Default: alphabetical by name
            };
        }

        private string GetStatusDisplayName(EmployeeStatus status)
        {
            return status switch
            {
                EmployeeStatus.Activa => "Activa",
                EmployeeStatus.Pendiente => "Pendiente",
                EmployeeStatus.Desactivada => "Desactivada",
                _ => "Desconocido"
            };
        }

        private void SeedSampleData()
        {
            // Add some initial sample data with employer ID = 1 for testing
            AddSampleEmployeesAsync(1).Wait();
        }

        private int GenerateId()
        {
            return _employees.Any() ? _employees.Max(e => e.Id) + 1 : 1;
        }

        // Methods to query PlaniFy.Empleado table
        public async Task<List<Empleado>> GetAllEmpleadosAsync()
        {
            try
            {
                var empleados = await _context.Empleados
                    .Include(e => e.Persona)
                    .Include(e => e.Empresa)
                    .ToListAsync();

                _logger.LogInformation("Retrieved {Count} empleados from PlaniFy.Empleado table", empleados.Count);
                return empleados;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving empleados from database");
                throw;
            }
        }

        public async Task<Empleado?> GetEmpleadoByIdAsync(int idPersona)
        {
            try
            {
                var empleado = await _context.Empleados
                    .Include(e => e.Persona)
                    .Include(e => e.Empresa)
                    .FirstOrDefaultAsync(e => e.idPersona == idPersona);

                _logger.LogInformation("Retrieved empleado with ID {IdPersona}", idPersona);
                return empleado;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving empleado with ID {IdPersona}", idPersona);
                throw;
            }
        }
    }
}
