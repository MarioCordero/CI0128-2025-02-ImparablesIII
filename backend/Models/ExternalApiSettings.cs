namespace backend.Models
{
    public class ExternalApiSettings
    {
        public PrivateInsuranceApi PrivateInsurance { get; set; } = new();
        public VoluntaryPensionApi VoluntaryPension { get; set; } = new();
        public SolidarityAssociationApi SolidarityAssociation { get; set; } = new();
    }

    public class PrivateInsuranceApi
    {
        public string BaseUrl { get; set; } = string.Empty;
        public string ApiToken { get; set; } = string.Empty;
    }

    public class VoluntaryPensionApi
    {
        public string BaseUrl { get; set; } = string.Empty;
        public string ApiToken { get; set; } = string.Empty;
    }

    public class SolidarityAssociationApi
    {
        public string BaseUrl { get; set; } = string.Empty;
        public string ApiToken { get; set; } = string.Empty;
    }
}
