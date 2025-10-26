using System.ComponentModel.DataAnnotations;

namespace backend.Models
{
    public class Employer
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; } = string.Empty;

        // Relationships
        public List<Company> Companies { get; set; } = new List<Company>();
    }

    public class Company
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(20)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        public string LegalId { get; set; } = string.Empty; // Cedula Juridica

        [Required]
        [MaxLength(20)]
        public string PayPeriod { get; set; } = string.Empty;

        public int EmployerId { get; set; }
        public Employer Employer { get; set; } = null!;

        public List<Employee> Employees { get; set; } = new List<Employee>();
        public List<Notification> Notifications { get; set; } = new List<Notification>();
        public List<Payroll> Payrolls { get; set; } = new List<Payroll>();
    }

    public class Employee
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string FullName { get; set; } = string.Empty;

        public bool Active { get; set; } = true;

        public int CompanyId { get; set; }

        public Company Company { get; set; } = null!;

        public Persona? Persona { get; set; }

    }

    public class Notification
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [MaxLength(500)]
        public string Message { get; set; } = string.Empty;

        public DateTime Date { get; set; } = DateTime.UtcNow;

        public int CompanyId { get; set; }
        public Company Company { get; set; } = null!;
    }

    // Deleted Payroll from this file to avoid conflict
    // public class Payroll
    // {
    //     public int Id { get; set; }
    //     public decimal Amount { get; set; }
    //     public DateTime Date { get; set; } = DateTime.UtcNow;

    //     public int CompanyId { get; set; }
    //     public Company Company { get; set; } = null!;
    // }
}