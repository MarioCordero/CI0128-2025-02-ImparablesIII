namespace backend.Services
{
    public interface IBenefitCodeParser
    {
        string GenerateBenefitCode(string benefitName, string deductionType);
        string ParseBenefitNameFromCode(string code);
    }
}

