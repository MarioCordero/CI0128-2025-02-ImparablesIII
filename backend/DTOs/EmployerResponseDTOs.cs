namespace backend.DTOs
{
    public class EmployerResponseDto
    {
        public int IdPersona { get; set; }
        public string Correo { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string? SegundoNombre { get; set; }
        public string Apellidos { get; set; } = string.Empty;
        public DateTime FechaNacimiento { get; set; }
        public string Cedula { get; set; } = string.Empty;
        public string? Rol { get; set; }
        public int? Telefono { get; set; }
        public int? IdDireccion { get; set; }
        public string TipoUsuario { get; set; } = string.Empty;
        public string Provincia { get; set; } = string.Empty;
        public string Canton { get; set; } = string.Empty;
        public string Distrito { get; set; } = string.Empty;
        public string? DireccionParticular { get; set; }
    }
}
