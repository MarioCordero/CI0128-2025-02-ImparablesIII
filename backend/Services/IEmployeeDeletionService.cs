using backend.DTOs;

namespace backend.Services
{
    public interface IEmployeeDeletionService
    {
        Task<DeleteEmployeeResponseDto> DeleteEmployeeAsync(int employeeId, int employerId, DeleteEmployeeRequestDto request);
        Task<EmployeeDeletionInfoDto> GetEmployeeDeletionInfoAsync(int employeeId);
        Task<bool> ValidateEmployerPasswordAsync(int employerId, string password);
    }
}

