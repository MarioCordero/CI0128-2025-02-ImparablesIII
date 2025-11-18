namespace backend.Constants
{
    public static class DeductionCodes
    {
        // Employee Deduction Codes
        public const string CcssSemEmployee = "CCSS_SEM_EE";
        public const string CcssIvmEmployee = "CCSS_IVM_EE";
        public const string BancoPopularEmployee = "BP_TRAB";
        public const string SalaryTax = "RENTA";

        // Employer Deduction Codes
        public const string CcssSemEmployer = "CCSS_SEM_ER";
        public const string CcssIvmEmployer = "CCSS_IVM_ER";
        public const string BancoPopularEmployer = "BP_PATRON";
        public const string InaEmployer = "INA_PATR";
        public const string FclEmployer = "FCL_PATR";
        public const string FodesafEmployer = "FODESAF_PATR";
        public const string FpcEmployer = "FPC_PATR";
    }

    public static class DeductionDisplayNames
    {
        public const string CcssSem = "SEM (Seguro Enfermedad/Maternidad)";
        public const string CcssIvm = "IVM (Invalidez, Vejez y Muerte)";
        public const string BancoPopularEmployee = "Aporte Trabajador Banco Popular";
        public const string SalaryTax = "Impuesto de renta";
    }
}

