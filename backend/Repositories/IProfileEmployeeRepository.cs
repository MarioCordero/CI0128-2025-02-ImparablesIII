using System.Threading.Tasks;
using backend.DTOs;

namespace backend.Repositories
{
    public interface IProfileEmployeeRepository
    {
        Task<ProfileEmployeeResponseDto> GetProfileByIdAsync(int employeeId);
        Task<bool> ExistsAsync(int employeeId);
        Task<bool> UpdateEmployeeProfileAsync(int employeeId, UpdateEmployeeProfileRequestDto updateRequest);
    }
}