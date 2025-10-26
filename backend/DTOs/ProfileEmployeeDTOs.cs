namespace backend.DTOs
{
    public class ProfileEmployeeResponseDto
    {
        public UserProfileDto User { get; set; } = new UserProfileDto();
        public ProjectInfoDto Project { get; set; } = new ProjectInfoDto();
    }

    public class UserProfileDto
    {
        public string Nombre { get; set; } = string.Empty;
        public string Puesto { get; set; } = string.Empty;
        public string Departamento { get; set; } = string.Empty;
        public string Correo { get; set; } = string.Empty;
        public int Telefono { get; set; }
        public string Direccion { get; set; } = string.Empty;
        public string IBAN { get; set; } = string.Empty;
        public string TipoContrato { get; set; } = string.Empty;
        public DateTime FechaContratacion { get; set; }
        public int Salario { get; set; }
    }

    public class ProjectInfoDto
    {
        public string NombreEmpresa { get; set; } = string.Empty;
        public string PeriodoPago { get; set; } = string.Empty;
    }

    public class UpdateEmployeeProfileRequestDto
    {
        public string Nombre { get; set; } = string.Empty;
        public string SegundoNombre { get; set; } = string.Empty;
        public string Apellidos { get; set; } = string.Empty;
        public int Telefono { get; set; }
        public string Provincia { get; set; } = string.Empty;
        public string Canton { get; set; } = string.Empty;
        public string Distrito { get; set; } = string.Empty;
        public string DireccionParticular { get; set; } = string.Empty;
        public string IBAN { get; set; } = string.Empty;
    }

    public class UpdateEmployeeProfileResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public UserProfileDto? UpdatedProfile { get; set; }
    }
}