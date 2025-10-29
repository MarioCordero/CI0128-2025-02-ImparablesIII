namespace backend.Constants
{
    public static class BenefitConstants
    {
        public const string BenefitTypeBonificacion = "bonificación";
        public const string BenefitTypeDescuento = "descuento";
        public const string BenefitTypeAmbos = "ambos";

        public const string DeductionTypeEmployer = "ER";
        public const string DeductionTypeEmployee = "EE";

        public const string CalculationTypeFixedAmount = "monto fijo";
        public const string CalculationTypePercentage = "porcentaje";
        public const string CalculationTypeApi = "api";
        public const int PercentageMultiplier = 100;
    }

    public static class BenefitNameKeywords
    {
        public const string Insurance = "seguro";
        public const string InsuranceEnglish = "insurance";
        public const string Association = "asociacion";
        public const string Solidarity = "solidaridad";
        public const string Pension = "pension";
        public const string Voluntary = "voluntaria";
    }

    public static class DeductionRoleNames
    {
        public const string EmployerDeduction = "EmployerDeduction";
        public const string EmployeeDeduction = "EmployeeDeduction";
    }
}

