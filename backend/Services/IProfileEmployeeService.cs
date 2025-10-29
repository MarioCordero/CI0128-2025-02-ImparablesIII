using System.Threading.Tasks;
using backend.DTOs;

namespace backend.Services
{
    public interface IProfileEmployeeService
    {
        Task<ProfileEmployeeResponseDto> GetEmployeeProfileAsync(int employeeId);
        Task<UpdateEmployeeProfileResponseDto> UpdateEmployeeProfileAsync(int employeeId, UpdateEmployeeProfileRequestDto updateRequest);
    }
}