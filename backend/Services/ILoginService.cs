using backend.DTOs;

namespace backend.Services
{
    public interface ILoginService
    {
        Task<LoginResponseDto> AuthenticateAsync(LoginRequestDto loginRequest);
        Task<UserDataDto?> GetUserDataAsync(int idPersona);
    }
}