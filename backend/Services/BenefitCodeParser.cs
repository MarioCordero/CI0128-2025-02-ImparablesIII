namespace backend.Services
{
    public class BenefitCodeParser : IBenefitCodeParser
    {
        public string GenerateBenefitCode(string benefitName, string deductionType)
        {
            var normalizedName = benefitName
                .ToUpper()
                .Replace(" ", "_")
                .Replace("-", "_");
            return $"{normalizedName}_{deductionType}";
        }

        public string ParseBenefitNameFromCode(string code)
        {
            if (string.IsNullOrEmpty(code))
            {
                return "Beneficio";
            }

            var parts = code.Split('_');
            if (parts.Length > 1)
            {
                var benefitNamePart = string.Join(" ", parts.Take(parts.Length - 1));
                return benefitNamePart.Replace("_", " ");
            }

            return code;
        }
    }
}

