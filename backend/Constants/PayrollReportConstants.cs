namespace backend.Constants
{
    public static class PayrollReportConstants
    {
        public static class ReportLabels
        {
            public const string ReportTitle = "REPORTE 1 – EMPLEADO (PAGO PLANILLA)";
            public const string CompanyNameLabel = "Nombre de la empresa";
            public const string EmployeeNameLabel = "Nombre completo del empleado";
            public const string PaymentDateLabel = "Fecha de pago";
            public const string ContractTypeLabel = "Tipo de contrato";
            public const string GrossSalaryLabel = "Salario Bruto";
            public const string ObligatoryDeductionsLabel = "Deducciones obligatorias";
            public const string VoluntaryDeductionsLabel = "Deducciones voluntarias";
            public const string TotalObligatoryDeductionsLabel = "Total deducciones obligatorias";
            public const string TotalVoluntaryDeductionsLabel = "Total deducciones voluntarias";
            public const string NetPayLabel = "Pago Neto";
        }

        public static class ReportFormatting
        {
            public const string DateFormat = "dd/MM/yyyy";
            public const string CurrencyFormat = "N2";
            public const string CultureCode = "es-CR";
            public const string CurrencySymbol = "₡";
            public const string NegativePrefix = "-";
        }

        public static class ReportLayout
        {
            public const int PageMarginCentimetres = 2;
            public const int DefaultSpacing = 20;
            public const int DefaultFontSize = 10;
            public const int HeaderFontSize = 18;
            public const int LabelFontSize = 9;
            public const int SectionFontSize = 11;
            public const int NetPayFontSize = 13;
            public const int LabelColumnWidth = 200;
            public const int AmountColumnWidth = 120;
            public const int VerticalPaddingSmall = 3;
            public const int VerticalPaddingMedium = 5;
            public const int VerticalPaddingLarge = 8;
            public const int VerticalPaddingExtraLarge = 15;
            public const int IndentationLeft = 15;
            public const int BorderWidth = 1;
            public const int BorderWidthThick = 2;
        }
    }
}

