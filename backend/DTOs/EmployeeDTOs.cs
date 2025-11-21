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

    public class DeleteEmployeeRequestDto
    {
        [System.Text.Json.Serialization.JsonPropertyName("contrasena")]
        public string Contrasena { get; set; } = string.Empty;
        
        [System.Text.Json.Serialization.JsonPropertyName("motivoBaja")]
        public string? MotivoBaja { get; set; }
    }

    public class DeleteEmployeeResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public bool IsLogicalDeletion { get; set; }
        public int PayrollRecordsCount { get; set; }
    }

    public class EmployeeDeletionInfoDto
    {
        public bool HasPayrollRecords { get; set; }
        public int PayrollRecordsCount { get; set; }
        public string EmployeeName { get; set; } = string.Empty;
    }
}