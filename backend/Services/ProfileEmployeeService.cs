using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using backend.DTOs;
using backend.Repositories;

namespace backend.Services
{
    public class ProfileEmployeeService : IProfileEmployeeService
    {
        private readonly IProfileEmployeeRepository _profileRepository;
        private readonly ILogger<ProfileEmployeeService> _logger;

        public ProfileEmployeeService(
            IProfileEmployeeRepository profileRepository,
            ILogger<ProfileEmployeeService> logger)
        {
            _profileRepository = profileRepository;
            _logger = logger;
        }

        public async Task<ProfileEmployeeResponseDto> GetEmployeeProfileAsync(int employeeId)
        {
            _logger.LogInformation("Iniciando obtención de perfil para empleado {EmployeeId}", employeeId);

            if (employeeId <= 0)
            {
                _logger.LogWarning("ID de empleado inválido: {EmployeeId}", employeeId);
                throw new ArgumentException("El ID del empleado debe ser mayor a 0");
            }

            var exists = await _profileRepository.ExistsAsync(employeeId);
            if (!exists)
            {
                _logger.LogWarning("Empleado no encontrado: {EmployeeId}", employeeId);
                throw new KeyNotFoundException($"No se encontró el empleado con ID {employeeId}");
            }

            var profile = await _profileRepository.GetProfileByIdAsync(employeeId);
            
            _logger.LogInformation("Perfil obtenido exitosamente para empleado {EmployeeId}", employeeId);
            return profile;
        }

        public async Task<UpdateEmployeeProfileResponseDto> UpdateEmployeeProfileAsync(int employeeId, UpdateEmployeeProfileRequestDto updateRequest)
        {
            _logger.LogInformation("Iniciando actualización de perfil para empleado {EmployeeId}", employeeId);

            if (employeeId <= 0)
            {
                _logger.LogWarning("ID de empleado inválido: {EmployeeId}", employeeId);
                return new UpdateEmployeeProfileResponseDto 
                { 
                    Success = false, 
                    Message = "El ID del empleado debe ser mayor a 0" 
                };
            }

            // Validar campos requeridos
            if (string.IsNullOrWhiteSpace(updateRequest.Nombre) || 
                string.IsNullOrWhiteSpace(updateRequest.Apellidos) ||
                string.IsNullOrWhiteSpace(updateRequest.Provincia) ||
                string.IsNullOrWhiteSpace(updateRequest.Canton) ||
                string.IsNullOrWhiteSpace(updateRequest.Distrito) ||
                string.IsNullOrWhiteSpace(updateRequest.DireccionParticular))
            {
                _logger.LogWarning("Campos requeridos faltantes para empleado {EmployeeId}", employeeId);
                return new UpdateEmployeeProfileResponseDto 
                { 
                    Success = false, 
                    Message = "Todos los campos son requeridos" 
                };
            }

            try
            {
                var exists = await _profileRepository.ExistsAsync(employeeId);
                if (!exists)
                {
                    _logger.LogWarning("Empleado no encontrado: {EmployeeId}", employeeId);
                    return new UpdateEmployeeProfileResponseDto 
                    { 
                        Success = false, 
                        Message = $"No se encontró el empleado con ID {employeeId}" 
                    };
                }

                var updateResult = await _profileRepository.UpdateEmployeeProfileAsync(employeeId, updateRequest);
                
                if (updateResult)
                {
                    _logger.LogInformation("Perfil actualizado exitosamente para empleado {EmployeeId}", employeeId);
                    
                    // Obtener el perfil actualizado
                    var updatedProfile = await _profileRepository.GetProfileByIdAsync(employeeId);
                    
                    return new UpdateEmployeeProfileResponseDto 
                    { 
                        Success = true, 
                        Message = "Perfil actualizado exitosamente",
                        UpdatedProfile = updatedProfile?.User
                    };
                }
                else
                {
                    _logger.LogWarning("No se pudo actualizar el perfil del empleado {EmployeeId}", employeeId);
                    return new UpdateEmployeeProfileResponseDto 
                    { 
                        Success = false, 
                        Message = "No se pudo actualizar el perfil" 
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error actualizando perfil del empleado {EmployeeId}", employeeId);
                return new UpdateEmployeeProfileResponseDto 
                { 
                    Success = false, 
                    Message = $"Error interno del servidor: {ex.Message}" 
                };
            }
        }
    }
}