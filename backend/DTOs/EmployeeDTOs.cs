namespace backend.DTOs
{
    public class EmployeeListDto
    {
        public int Id { get; set; }
        public string NombreCompleto { get; set; } = string.Empty;
        public string Correo { get; set; } = string.Empty;
        public int? Telefono { get; set; }
        public string Puesto { get; set; } = string.Empty;
        public string Departamento { get; set; } = string.Empty;
        public int Salario { get; set; }
        public string TipoContrato { get; set; } = string.Empty;
    }

    public class EmployeeListResponseDto
    {
        public List<EmployeeListDto> Employees { get; set; } = new List<EmployeeListDto>();
        public int TotalCount { get; set; }
    }
}