using System;
using System.ComponentModel.DataAnnotations;

namespace backend.Models
{
    public class ProfileEmployee
    {
        [Key]
        public int IdPersona { get; set; }

        // Persona
        [Required]
        [MaxLength(62)]
        public string NombreCompleto { get; set; } = string.Empty;

        [Required]
        [MaxLength(30)]
        public string CorreoElectronico { get; set; } = string.Empty;

        [Required]
        public int NumeroTelefono { get; set; }

        // Empleado
        [Required]
        [MaxLength(30)]
        public string IBAN { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        public string Departamento { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        public string Puesto { get; set; } = string.Empty;

        [Required]
        [MaxLength(25)]
        public string TipoContrato { get; set; } = string.Empty;

        [Required]
        public DateTime FechaContratacion { get; set; }

        [Required]
        public int Salario { get; set; }

        // Direccion
        [Required]
        public string DireccionCompleta { get; set; } = string.Empty;

        // Información de la empresa
        [Required]
        [MaxLength(20)]
        public string NombreEmpresa { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        public string PeriodoPago { get; set; } = string.Empty;

    }
}