using backend.Services.PaymentsCalculate;
using backend.Models;
using backend.DTOs;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace backend.Services.PaymentsCalculate.Benefits
{
    public class VoluntaryPension
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<VoluntaryPension> _logger;
        private readonly ExternalApiSettings _apiSettings;

        public VoluntaryPension(HttpClient httpClient, ILogger<VoluntaryPension> logger, IOptions<ExternalApiSettings> apiSettings)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _apiSettings = apiSettings?.Value ?? throw new ArgumentNullException(nameof(apiSettings));
        }

        public async Task<List<DeductionItem>> CalculateAsync(decimal grossSalary, string PensionType)
        {
            if (grossSalary <= 0)
            {
                _logger.LogWarning("Invalid gross salary: {Salary}", grossSalary);
                throw new ArgumentException("Gross salary must be greater than 0", nameof(grossSalary));
            }

            if (string.IsNullOrEmpty(PensionType) || !new[] { "A", "B", "C" }.Contains(PensionType.ToUpper()))
            {
                _logger.LogWarning("Invalid pension type: {Type}", PensionType);
                throw new ArgumentException("Pension type must be A, B, or C", nameof(PensionType));
            }

            try
            {
                _logger.LogInformation("Calculating voluntary pension for salary: {Salary}, type: {Type}", 
                    grossSalary, PensionType);

                var deductions = await GetPensionVoluntariaAsync(grossSalary, PensionType.ToUpper());
                
                _logger.LogInformation("Voluntary pension calculation completed. Found {Count} deductions", deductions.Count);
                
                return deductions;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calculating voluntary pension for salary: {Salary}, type: {Type}", 
                    grossSalary, PensionType);
                throw new InvalidOperationException("Failed to calculate voluntary pension deduction", ex);
            }
        }

        private async Task<List<DeductionItem>> GetPensionVoluntariaAsync(decimal salary, string pensionType)
        {
            try
            {
                var responseContent = await CallVoluntaryPensionApiAsync(salary, pensionType);
                var apiResponse = DeserializeApiResponse(responseContent);
                return apiResponse.Deductions;
            }
            catch (HttpRequestException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error calling voluntary pension API");
                throw new InvalidOperationException("Failed to call voluntary pension API", ex);
            }
        }

        private async Task<string> CallVoluntaryPensionApiAsync(decimal salary, string pensionType)
        {
            _httpClient.DefaultRequestHeaders.Clear();
            
            var url = $"{_apiSettings.VoluntaryPension.BaseUrl}/?planType={pensionType}&grossSalary={salary}";
            
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
                    throw new InvalidOperationException("Invalid response format from pension API");
                }

                return apiResponse;
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Failed to deserialize API response: {Response}", responseContent);
                throw new InvalidOperationException("Invalid JSON response from voluntary pension API", ex);
            }
        }
    }
}
