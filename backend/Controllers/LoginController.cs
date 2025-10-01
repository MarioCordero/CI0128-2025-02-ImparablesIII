using Microsoft.AspNetCore.Mvc;
using backend.DTOs;
using backend.Services;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LoginController : ControllerBase
    {
        private readonly ILoginService _loginService;

        public LoginController(ILoginService loginService)
        {
            _loginService = loginService;
        }

        [HttpPost]
        public async Task<ActionResult<LoginResponseDto>> Login([FromBody] LoginRequestDto loginRequest)
        {
            if (string.IsNullOrEmpty(loginRequest.Correo) || string.IsNullOrEmpty(loginRequest.Contrasena))
            {
                return BadRequest(new LoginResponseDto
                {
                    Success = false,
                    Message = "Correo y contraseña son requeridos"
                });
            }

            var result = await _loginService.AuthenticateAsync(loginRequest);
            
            if (!result.Success)
            {
                return Unauthorized(result);
            }

            return Ok(result);
        }

        [HttpGet("user/{idPersona}")]
        public async Task<ActionResult<UserDataDto>> GetUserData(int idPersona)
        {
            var userData = await _loginService.GetUserDataAsync(idPersona);
            
            if (userData == null)
            {
                return NotFound("Usuario no encontrado");
            }

            return Ok(userData);
        }
    }
}