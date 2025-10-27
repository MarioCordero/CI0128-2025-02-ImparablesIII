namespace backend.Services.PaymentsCalculate
{
    public enum CalcRole { EmployeeDeduction, EmployerDeduction }

    public readonly record struct CalcLine(string Code, decimal Amount, CalcRole Role)
    {
        public override string ToString() => $"{Code} -> {Amount:0.00} ({Role})";
    }
}