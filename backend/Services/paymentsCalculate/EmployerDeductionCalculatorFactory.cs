using backend.DTOs;
using backend.Services.PaymentsCalculate.Employer;
using backend.Constants;

namespace backend.Services.PaymentsCalculate
{
    public class EmployerDeductionCalculatorFactory
    {
        public IEmployerDeductionCalculator? CreateCalculator(EmployerDeductionDto deduction)
        {
            return deduction.Code switch
            {
                DeductionCodes.CcssSemEmployer => new CcssSemEmployerCalc(deduction.Rate),
                DeductionCodes.CcssIvmEmployer => new CcssIvmEmployerCalc(deduction.Rate),
                DeductionCodes.BancoPopularEmployer => new BancoPopularEmployerCalc(deduction.Rate),
                DeductionCodes.InaEmployer => new InaEmployerCalc(deduction.Rate),
                DeductionCodes.FclEmployer => new FclEmployerCalc(deduction.Rate),
                DeductionCodes.FodesafEmployer => new FodesafEmployerCalc(deduction.Rate),
                DeductionCodes.FpcEmployer => new FpcEmployerCalc(deduction.Rate),
                _ => null
            };
        }
    }
}

