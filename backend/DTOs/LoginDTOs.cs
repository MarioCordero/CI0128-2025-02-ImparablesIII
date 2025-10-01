namespace backend.DTOs
{
    public class LoginRequestDto
    {
        public string Correo { get; set; } = string.Empty;
        public string Contrasena { get; set; } = string.Empty;
    }

    public class LoginResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public UserDataDto? UserData { get; set; }
        public string? Token { get; set; }
    }

    public class UserDataDto
    {
        public int IdPersona { get; set; }
        public string Correo { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string? SegundoNombre { get; set; }
        public string Apellidos { get; set; } = string.Empty;
        public string TipoUsuario { get; set; } = string.Empty;
        public string? Departamento { get; set; }
        public string? Puesto { get; set; }
        public int? IdEmpresa { get; set; }
    }
}