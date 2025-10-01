
public class Employer
{
    public int Id { get; set; }
    public string Name { get; set; }

    // Relationships
    public List<Company> Companies { get; set; }
}

public class Company
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string LegalId { get; set; } // Cedula Juridica
    public string PayPeriod { get; set; }

    public int EmployerId { get; set; }
    public Employer Employer { get; set; }

    public List<Employee> Employees { get; set; }
    public List<Notification> Notifications { get; set; }
    public List<Payroll> Payrolls { get; set; }
}

public class Employee
{
    public int Id { get; set; }
    public string FullName { get; set; }
    public bool Active { get; set; }

    public int CompanyId { get; set; }
    public Company Company { get; set; }
}

public class Notification
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Message { get; set; }
    public DateTime Date { get; set; }

    public int CompanyId { get; set; }
    public Company Company { get; set; }
}

public class Payroll
{
    public int Id { get; set; }
    public decimal Amount { get; set; }
    public DateTime Date { get; set; }

    public int CompanyId { get; set; }
    public Company Company { get; set; }
}
