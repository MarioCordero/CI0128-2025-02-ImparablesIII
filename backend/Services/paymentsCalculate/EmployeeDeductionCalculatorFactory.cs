using backend.DTOs;
using backend.Services.PaymentsCalculate.Employee;
using backend.Constants;

namespace backend.Services.PaymentsCalculate
{
    public class EmployeeDeductionCalculatorFactory
    {
        public IEmployeeDeductionCalculator? CreateCalculator(EmployeeDeductionDto deduction)
        {
            return deduction.Code switch
            {
                DeductionCodes.CcssSemEmployee => new CcssSemEmployeeCalc(deduction.Rate),
                DeductionCodes.CcssIvmEmployee => new CcssIvmEmployeeCalc(deduction.Rate),
                DeductionCodes.BancoPopularEmployee => new BancoPopularEmployeeCalc(deduction.Rate),
                DeductionCodes.SalaryTax => new SalaryTaxEmployeeCalc(deduction.Rate, deduction.MinAmount, deduction.MaxAmount),
                _ => null
            };
        }

        public string GetDisplayName(string code)
        {
            return code switch
            {
                DeductionCodes.CcssSemEmployee => DeductionDisplayNames.CcssSem,
                DeductionCodes.CcssIvmEmployee => DeductionDisplayNames.CcssIvm,
                DeductionCodes.BancoPopularEmployee => DeductionDisplayNames.BancoPopularEmployee,
                DeductionCodes.SalaryTax => DeductionDisplayNames.SalaryTax,
                _ => string.Empty
            };
        }
    }
}

