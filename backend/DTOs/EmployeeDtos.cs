namespace backend.DTOs
{
    public class EmployeeListDto
    {
        public int Id { get; set; }
        public string NombreCompleto { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Celular { get; set; } = string.Empty;
        public string Puesto { get; set; } = string.Empty;
        public decimal Salario { get; set; }
        public string Estado { get; set; } = string.Empty;
        public DateTime FechaContratacion { get; set; }
    }
    
    public class EmployeeFilterDto
    {
        public string? SearchTerm { get; set; }
        public string? EstadoFilter { get; set; }
        public string? SortBy { get; set; } = "nombre"; // Default sort by name
        public string? SortDirection { get; set; } = "asc"; // Default ascending
    }
    
    public class EmployeeListResponseDto
    {
        public List<EmployeeListDto> Employees { get; set; } = new();
        public int TotalCount { get; set; }
        public string Message { get; set; } = string.Empty;
        public bool HasEmployees => Employees.Any();
    }
}
