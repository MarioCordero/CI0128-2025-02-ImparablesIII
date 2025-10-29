using backend.Services.PaymentsCalculate;
using backend.Models;
using backend.DTOs;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace backend.Services.PaymentsCalculate.Benefits
{
  public class SolidarityAssociation
  {
    private readonly HttpClient _httpClient;
    private readonly ILogger<SolidarityAssociation> _logger;
    private readonly ExternalApiSettings _apiSettings;

    public SolidarityAssociation(HttpClient httpClient, ILogger<SolidarityAssociation> logger, IOptions<ExternalApiSettings> apiSettings)
    {
      _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
      _logger = logger ?? throw new ArgumentNullException(nameof(logger));
      _apiSettings = apiSettings?.Value ?? throw new ArgumentNullException(nameof(apiSettings));
    }

    public async Task<List<DeductionItem>> CalculateAsync(string companyId, decimal grossSalary)
    {
      if (string.IsNullOrWhiteSpace(companyId))
      {
        _logger.LogWarning("Invalid company ID: {CompanyId}", companyId);
        throw new ArgumentException("Company ID cannot be null or empty", nameof(companyId));
      }

      if (grossSalary <= 0)
      {
        _logger.LogWarning("Invalid gross salary: {Salary}", grossSalary);
        throw new ArgumentException("Gross salary must be greater than 0", nameof(grossSalary));
      }

      try
      {
        _logger.LogInformation("Calculating solidarity association for company: {CompanyId}, salary: {Salary}",
            companyId, grossSalary);

        var deductions = await GetSolidarityAssociationAsync(companyId, grossSalary);

        _logger.LogInformation("Solidarity association calculation completed. Found {Count} deductions", deductions.Count);

        return deductions;
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Error calculating solidarity association for company: {CompanyId}, salary: {Salary}",
            companyId, grossSalary);
        throw new InvalidOperationException("Failed to calculate solidarity association deduction", ex);
      }
    }

        private async Task<List<DeductionItem>> GetSolidarityAssociationAsync(string companyId, decimal grossSalary)
        {
            try
            {
                var responseContent = await CallSolidarityAssociationApiAsync(companyId, grossSalary);
                var apiResponse = DeserializeApiResponse(responseContent);
                return apiResponse.Deductions;
            }
            catch (HttpRequestException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error calling solidarity association API");
                throw new InvalidOperationException("Failed to call solidarity association API", ex);
            }
        }

        private async Task<string> CallSolidarityAssociationApiAsync(string companyId, decimal grossSalary)
        {
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("Authorization", _apiSettings.SolidarityAssociation.ApiToken);

            var url = $"{_apiSettings.SolidarityAssociation.BaseUrl}api/asociacionsolidarista/aporte-empleado?cedulaEmpresa={companyId}&salarioBruto={grossSalary}";

            _logger.LogDebug("Making API call to: {Url}", url);

            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("API call failed with status: {StatusCode}", response.StatusCode);
                throw new HttpRequestException($"API call failed with status code: {response.StatusCode}");
            }

            var responseContent = await response.Content.ReadAsStringAsync();
            _logger.LogDebug("API response: {Response}", responseContent);

            return responseContent;
        }

        private ExternalApiDeductionResponse DeserializeApiResponse(string responseContent)
        {
            try
            {
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };
                
                var apiResponse = JsonSerializer.Deserialize<ExternalApiDeductionResponse>(responseContent, options);
                
                if (apiResponse?.Deductions == null || !apiResponse.Deductions.Any())
                {
                    _logger.LogError("Invalid API response format: {Response}", responseContent);
                    throw new InvalidOperationException("Invalid response format from solidarity association API");
                }

                return apiResponse;
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Failed to deserialize API response: {Response}", responseContent);
                throw new InvalidOperationException("Invalid JSON response from solidarity association API", ex);
            }
        }
  }
}
