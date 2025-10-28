using backend.Services.PaymentsCalculate;
using backend.Models;
using backend.DTOs;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace backend.Services.PaymentsCalculate.Benefits
{
  public class PrivateInsurance
  {
    private const int DEPENDENTS_CONSTANT = 2;
    private const string BENEFIT_CODE = "SEG_PRIV";
    private readonly HttpClient _httpClient;
    private readonly ILogger<PrivateInsurance> _logger;
    private readonly ExternalApiSettings _apiSettings;

    public PrivateInsurance(HttpClient httpClient, ILogger<PrivateInsurance> logger, IOptions<ExternalApiSettings> apiSettings)
    {
      _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
      _logger = logger ?? throw new ArgumentNullException(nameof(logger));
      _apiSettings = apiSettings?.Value ?? throw new ArgumentNullException(nameof(apiSettings));
    }
        public async Task<List<CalcLine>> CalculateAsync(int employeeAge, int dependents)
        {
            if (employeeAge <= 0)
            {
                _logger.LogWarning("Invalid employee age: {Age}", employeeAge);
                throw new ArgumentException("Employee age must be greater than 0", nameof(employeeAge));
            }

            if (dependents < 0)
            {
                _logger.LogWarning("Invalid number of dependents: {Dependents}", dependents);
                throw new ArgumentException("Number of dependents must be greater than or equal to 0", nameof(dependents));
            }

            try
            {
                _logger.LogInformation("Calculating private insurance for employee age: {Age}, dependents: {Dependents}", 
                    employeeAge, dependents);

                var deductions = await GetPrivateInsuranceAsync(employeeAge, dependents);
                
                _logger.LogInformation("Private insurance calculation completed. Found {Count} deductions", deductions.Count);
                
                return deductions;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calculating private insurance for employee age: {Age}", employeeAge);
                throw new InvalidOperationException("Failed to calculate private insurance deduction", ex);
            }
        }

    private async Task<List<CalcLine>> GetPrivateInsuranceAsync(int age, int dependents)
    {
        try
        {
            var responseContent = await CallPrivateInsuranceApiAsync(age, dependents);
            var apiResponse = DeserializeApiResponse(responseContent);
            return ConvertToCalcLines(apiResponse);
        }
        catch (HttpRequestException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error calling private insurance API");
            throw new InvalidOperationException("Failed to call private insurance API", ex);
        }
    }

    private async Task<string> CallPrivateInsuranceApiAsync(int age, int dependents)
    {
        _httpClient.DefaultRequestHeaders.Clear();
        _httpClient.DefaultRequestHeaders.Add("Authorization", _apiSettings.PrivateInsurance.ApiToken);

        var url = $"{_apiSettings.PrivateInsurance.BaseUrl}/SeguroPrivado/seguro-privado?edad={age}&dependientes={dependents}";

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
            var apiResponse = JsonSerializer.Deserialize<ExternalApiDeductionResponse>(responseContent);
            
            if (apiResponse?.Deductions == null || !apiResponse.Deductions.Any())
            {
                _logger.LogError("Invalid API response format: {Response}", responseContent);
                throw new InvalidOperationException("Invalid response format from private insurance API");
            }

            return apiResponse;
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Failed to deserialize API response");
            throw new InvalidOperationException("Invalid JSON response from private insurance API", ex);
        }
    }

    private List<CalcLine> ConvertToCalcLines(ExternalApiDeductionResponse apiResponse)
    {
        var calcLines = new List<CalcLine>();
        
        foreach (var deduction in apiResponse.Deductions)
        {
            var calcRole = MapDeductionTypeToCalcRole(deduction.Type);
            var calcLine = new CalcLine($"{BENEFIT_CODE}_{deduction.Type}", Math.Round(deduction.Amount, 2), calcRole);
            calcLines.Add(calcLine);
        }
        
        _logger.LogInformation("Private insurance API returned {Count} deductions", calcLines.Count);
        return calcLines;
    }

    private static CalcRole MapDeductionTypeToCalcRole(string deductionType)
    {
        return deductionType.ToUpper() switch
        {
            "ER" => CalcRole.EmployerDeduction,
            "EE" => CalcRole.EmployeeDeduction,
        };
    }
  }
}
