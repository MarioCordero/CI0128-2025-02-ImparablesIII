using System.ComponentModel.DataAnnotations;

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
        public int? Salario { get; set; }
        public string TipoContrato { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
    }

    public class EmployeeListResponseDto
    {
        public List<EmployeeListDto> Employees { get; set; } = new List<EmployeeListDto>();
        public int TotalCount { get; set; }
    }

    public class DeleteEmployeeRequestDto
    {
        [System.Text.Json.Serialization.JsonPropertyName("employerId")]
        public int EmployerId { get; set; }

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

    public class RegisterEmployeeResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public int? EmployeeId { get; set; }
        public IEnumerable<string>? ValidationErrors { get; set; }
    }

    public class RegisterEmployeeDto
    {
        // Personal Information
        [Required]
        [MaxLength(20)]
        public string PrimerNombre { get; set; } = string.Empty;
        
        [MaxLength(20)]
        public string? SegundoNombre { get; set; }
        
        [Required]
        [MaxLength(20)]
        public string PrimerApellido { get; set; } = string.Empty;
        
        [Required]
        public DateTime FechaNacimiento { get; set; }
        
        [Required]
        [MaxLength(9)]
        public string Cedula { get; set; } = string.Empty;
        
        [Required]
        public int Telefono { get; set; }
        
        [Required]
        [EmailAddress]
        [MaxLength(50)]
        public string Correo { get; set; } = string.Empty;
        
        // Address Information
        [Required]
        [MaxLength(12)]
        public string Provincia { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(30)]
        public string Canton { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(30)]
        public string Distrito { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(150)]
        public string DireccionParticular { get; set; } = string.Empty;
        
        // Employment Information
        [Required]
        [MaxLength(20)]
        public string Departamento { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(20)]
        public string TipoContrato { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(20)]
        public string Puesto { get; set; } = string.Empty;
        
        [Required]
        public int Salario { get; set; }
        
        [Required]
        [MaxLength(30)]
        public string NumeroCuentaIban { get; set; } = string.Empty;
        
        [MaxLength(16)]
        public string? Contrasena { get; set; }
        
        // Relationship IDs
        public int? IdEmpresa { get; set; }
        public int? EmployerId { get; set; }
        public int? ProjectId { get; set; }
    }
}