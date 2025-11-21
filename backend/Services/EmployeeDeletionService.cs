using backend.DTOs;
using backend.Repositories;
using backend.Constants;
using Microsoft.Extensions.Logging;

namespace backend.Services
{
    public class EmployeeDeletionService : IEmployeeDeletionService
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly ILogger<EmployeeDeletionService> _logger;

        public EmployeeDeletionService(
            IEmployeeRepository employeeRepository,
            IUsuarioRepository usuarioRepository,
            ILogger<EmployeeDeletionService> logger)
        {
            _employeeRepository = employeeRepository;
            _usuarioRepository = usuarioRepository;
            _logger = logger;
        }

        public async Task<DeleteEmployeeResponseDto> DeleteEmployeeAsync(
            int employeeId, 
            int employerId, 
            DeleteEmployeeRequestDto request)
        {
            try
            {
                _logger.LogInformation("Iniciando eliminación de empleado {EmployeeId} por empleador {EmployerId}", employeeId, employerId);

                var validationResult = await ValidateDeletionRequestAsync(employeeId, employerId, request);
                if (validationResult != null)
                {
                    return validationResult;
                }

                return await ProcessEmployeeDeletionAsync(employeeId, employerId, request);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar empleado {EmployeeId}", employeeId);
                return CreateErrorResponse($"Error interno del servidor: {ex.Message}");
            }
        }

        public async Task<EmployeeDeletionInfoDto> GetEmployeeDeletionInfoAsync(int employeeId)
        {
            try
            {
                _logger.LogInformation("Obteniendo información de eliminación para empleado {EmployeeId}", employeeId);
                return await _employeeRepository.GetEmployeeDeletionInfoAsync(employeeId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener información de eliminación para empleado {EmployeeId}", employeeId);
                throw;
            }
        }

        public async Task<bool> ValidateEmployerPasswordAsync(int employerId, string password)
        {
            try
            {
                var user = await _usuarioRepository.GetUserByIdAsync(employerId);
                if (user == null)
                {
                    _logger.LogWarning("Usuario {EmployerId} no encontrado para validación de contraseña", employerId);
                    return false;
                }

                return password == user.Contrasena;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al validar contraseña para empleador {EmployerId}", employerId);
                return false;
            }
        }

        private async Task<DeleteEmployeeResponseDto?> ValidateDeletionRequestAsync(
            int employeeId, 
            int employerId, 
            DeleteEmployeeRequestDto request)
        {
            var employee = await _employeeRepository.GetEmployeeByIdAsync(employeeId);
            if (employee == null)
            {
                _logger.LogWarning("Empleado {EmployeeId} no encontrado", employeeId);
                return CreateErrorResponse(ReturnMessagesConstants.Employee.EmployeeNotFound);
            }

            var isPasswordValid = await ValidateEmployerPasswordAsync(employerId, request.Contrasena);
            if (!isPasswordValid)
            {
                _logger.LogWarning("Contraseña inválida para empleador {EmployerId}", employerId);
                return CreateErrorResponse(ReturnMessagesConstants.Employee.InvalidPassword);
            }

            return null;
        }

        private async Task<DeleteEmployeeResponseDto> ProcessEmployeeDeletionAsync(
            int employeeId, 
            int employerId, 
            DeleteEmployeeRequestDto request)
        {
            var hasPayrollRecords = await _employeeRepository.HasPayrollRecordsAsync(employeeId);
            
            if (hasPayrollRecords)
            {
                return await PerformLogicalDeletionAsync(employeeId, employerId, request);
            }
            else
            {
                return await PerformPhysicalDeletionAsync(employeeId);
            }
        }

        private async Task<DeleteEmployeeResponseDto> PerformLogicalDeletionAsync(
            int employeeId, 
            int employerId, 
            DeleteEmployeeRequestDto request)
        {
            var payrollCount = await _employeeRepository.GetPayrollRecordsCountAsync(employeeId);
            _logger.LogInformation("Empleado {EmployeeId} tiene {Count} registros de planilla. Realizando eliminación lógica.", employeeId, payrollCount);
            
            var motivoBaja = request.MotivoBaja ?? ReturnMessagesConstants.Employee.DefaultDeletionReason;
            var success = await _employeeRepository.DeleteEmployeeLogicallyAsync(employeeId, employerId, motivoBaja);

            if (success)
            {
                _logger.LogInformation("Eliminación lógica exitosa para empleado {EmployeeId}", employeeId);
                return CreateLogicalDeletionSuccessResponse(payrollCount);
            }
            else
            {
                _logger.LogError("Error al realizar eliminación lógica para empleado {EmployeeId}", employeeId);
                return CreateErrorResponse(ReturnMessagesConstants.Employee.DeletionError);
            }
        }

        private async Task<DeleteEmployeeResponseDto> PerformPhysicalDeletionAsync(int employeeId)
        {
            _logger.LogInformation("Empleado {EmployeeId} no tiene registros de planilla. Realizando eliminación física.", employeeId);
            
            var success = await _employeeRepository.DeleteEmployeePhysicallyAsync(employeeId);

            if (success)
            {
                _logger.LogInformation("Eliminación física exitosa para empleado {EmployeeId}", employeeId);
                return CreatePhysicalDeletionSuccessResponse();
            }
            else
            {
                _logger.LogError("Error al realizar eliminación física para empleado {EmployeeId}", employeeId);
                return CreateErrorResponse(ReturnMessagesConstants.Employee.DeletionError);
            }
        }

        private DeleteEmployeeResponseDto CreateErrorResponse(string message)
        {
            return new DeleteEmployeeResponseDto
            {
                Success = false,
                Message = message
            };
        }

        private DeleteEmployeeResponseDto CreateLogicalDeletionSuccessResponse(int payrollCount)
        {
            return new DeleteEmployeeResponseDto
            {
                Success = true,
                Message = string.Format(ReturnMessagesConstants.Employee.LogicalDeletionSuccess, payrollCount),
                IsLogicalDeletion = true,
                PayrollRecordsCount = payrollCount
            };
        }

        private DeleteEmployeeResponseDto CreatePhysicalDeletionSuccessResponse()
        {
            return new DeleteEmployeeResponseDto
            {
                Success = true,
                Message = ReturnMessagesConstants.Employee.PhysicalDeletionSuccess,
                IsLogicalDeletion = false,
                PayrollRecordsCount = 0
            };
        }
    }
}

