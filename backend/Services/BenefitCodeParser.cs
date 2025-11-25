using System;
using System.Linq;

namespace backend.Services
{
    public class BenefitCodeParser : IBenefitCodeParser
    {
        public string GenerateBenefitCode(string benefitName, string deductionType)
        {
            if (string.IsNullOrWhiteSpace(benefitName))
                throw new ArgumentException("El nombre del beneficio es requerido.", nameof(benefitName));

            if (string.IsNullOrWhiteSpace(deductionType))
                throw new ArgumentException("El tipo de deducción es requerido.", nameof(deductionType));

            var normalizedName = benefitName
                .Trim()
                .ToUpper()
                .Replace(" ", "_")
                .Replace("-", "_");

            var normalizedType = deductionType.Trim().ToUpper();

            return $"{normalizedName}_{normalizedType}";
        }

        public string ParseBenefitNameFromCode(string code)
        {
            if (string.IsNullOrWhiteSpace(code))
                return "Beneficio";

            var parts = code.Split('_', StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length <= 1)
                return parts[0];
                
            var nameParts = parts.Take(parts.Length - 1);
            var result = string.Join(" ", nameParts);
            return result;
        }
    }
}