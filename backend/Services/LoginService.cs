using backend.DTOs;
using backend.Repositories;
using System.Text;

namespace backend.Services
{
    public class LoginService : ILoginService
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IConfiguration _configuration;

        public LoginService(IUsuarioRepository usuarioRepository, IConfiguration configuration)
        {
            _usuarioRepository = usuarioRepository;
            _configuration = configuration;
        }

        public async Task<LoginResponseDto> AuthenticateAsync(LoginRequestDto loginRequest)
        {
            try
            {
                // Get user by email
                var user = await _usuarioRepository.GetUserByEmailAsync(loginRequest.Correo);
                
                if (user == null)
                {
                    return new LoginResponseDto
                    {
                        Success = false,
                        Message = "Usuario no encontrado"
                    };
                }

                // Verify password
                if (!VerifyPassword(loginRequest.Contrasena, user.Contrasena))
                {
                    return new LoginResponseDto
                    {
                        Success = false,
                        Message = "Contraseña incorrecta"
                    };
                }

                // Get user data
                var userData = await GetUserDataAsync(user.IdPersona);
                
                if (userData == null)
                {
                    return new LoginResponseDto
                    {
                        Success = false,
                        Message = "Error al obtener datos del usuario"
                    };
                }

                return new LoginResponseDto
                {
                    Success = true,
                    Message = "Login exitoso",
                    UserData = userData,
                    Token = GenerateToken(user.IdPersona.ToString())
                };
            }
            catch (Exception ex)
            {
                return new LoginResponseDto
                {
                    Success = false,
                    Message = $"Error interno del servidor: {ex.Message}"
                };
            }
        }

        public async Task<UserDataDto?> GetUserDataAsync(int idPersona)
        {
            try
            {
                return await _usuarioRepository.GetUserDataAsync(idPersona);
            }
            catch
            {
                return null;
            }
        }

        private bool VerifyPassword(string inputPassword, string storedPassword)
        {
            return inputPassword == storedPassword;
        }

        private string GenerateToken(string userId)
        {
            var token = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{userId}:{DateTime.UtcNow}"));
            return token;
        }
    }
}