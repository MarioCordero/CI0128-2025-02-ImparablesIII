using System.ComponentModel.DataAnnotations;

namespace backend_lab_c28730.Models
{
    public class Company
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(80, MinimumLength = 1)]
        public string CompanyName { get; set; } = string.Empty;
        
        [Required]
        [StringLength(9, MinimumLength = 9)]
        public string LegalId { get; set; } = string.Empty; // Cédula jurídica (9 digits)
        
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        
        [StringLength(200)]
        public string? Address { get; set; }
        
        public string? Phone { get; set; }
        
        [Required]
        public int MaxBenefits { get; set; }
        
        [Required]
        public string PaymentPeriod { get; set; } = string.Empty; // "quincenal" or "mensual"
        
        public int EmployerId { get; set; } // Foreign key to the employer who created this company
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}