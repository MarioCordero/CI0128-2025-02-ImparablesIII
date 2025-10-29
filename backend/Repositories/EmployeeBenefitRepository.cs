using System;
using System.Reflection;
using backend.DTOs;
using Microsoft.Data.SqlClient;
using Dapper;

namespace backend.Repositories
{
    public class EmployeeBenefitRepository : IEmployeeBenefitRepository
    {
        private const int DEFAULT_MAX_BENEFITS = 3;
        private const string STRING_NULL_MESSAGE = "Error mapping benefit row";
        private const string BENEFIT_ADDED_MESSAGE = "Beneficio agregado exitosamente";
        private const string BENEFIT_ADD_ERROR = "Error adding benefit to employee";
        
        private readonly string _connectionString;
        private readonly ILogger<EmployeeBenefitRepository> _logger;

        public EmployeeBenefitRepository(IConfiguration configuration, ILogger<EmployeeBenefitRepository> logger)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection") ?? "";
            _logger = logger;
        }

        public async Task<List<EmployeeBenefitDto>> GetAvailableBenefitsForEmployeeAsync(int employeeId, int companyId, BenefitFilterDto? filter = null)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var parameters = new DynamicParameters();
            parameters.Add("EmployeeId", employeeId);
            parameters.Add("CompanyId", companyId);
            parameters.Add("SearchTerm", filter?.SearchTerm);
            parameters.Add("CalculationType", filter?.CalculationType);
            parameters.Add("Status", filter?.Status == "Disponible" ? "Disponible" : null);

            var result = await connection.QueryAsync<dynamic>("PlaniFy.GetEmployeeBenefitsSummary", parameters, commandType: System.Data.CommandType.StoredProcedure);
            var benefits = MapBenefitsFromStoredProcedure(result);
            
            return benefits.Where(b => !b.IsSelected).ToList();
        }

        public async Task<List<EmployeeBenefitDto>> GetSelectedBenefitsForEmployeeAsync(int employeeId, int companyId)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var parameters = new DynamicParameters();
            parameters.Add("EmployeeId", employeeId);
            parameters.Add("CompanyId", companyId);
            parameters.Add("SearchTerm", null);
            parameters.Add("CalculationType", null);
            parameters.Add("Status", "Seleccionado");

            var result = await connection.QueryAsync<dynamic>("PlaniFy.GetEmployeeBenefitsSummary", parameters, commandType: System.Data.CommandType.StoredProcedure);
            var benefits = MapBenefitsFromStoredProcedure(result);
            
            return benefits.Where(b => b.IsSelected).ToList();
        }

        public async Task<bool> IsBenefitSelectedAsync(int employeeId, int companyId, string benefitName)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var query = @"
                SELECT COUNT(1)
                FROM PlaniFy.BeneficioEmpleado
                WHERE idEmpleado = @EmployeeId AND idEmpresa = @CompanyId AND NombreBeneficio = @BenefitName";

            var parameters = new { EmployeeId = employeeId, CompanyId = companyId, BenefitName = benefitName };

            var count = await connection.QuerySingleAsync<int>(query, parameters);
            return count > 0;
        }

        public async Task<int> GetSelectedBenefitsCountAsync(int employeeId, int companyId)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var query = @"
                SELECT COUNT(1)
                FROM PlaniFy.BeneficioEmpleado
                WHERE idEmpleado = @EmployeeId AND idEmpresa = @CompanyId";

            var parameters = new { EmployeeId = employeeId, CompanyId = companyId };

            var count = await connection.QuerySingleAsync<int>(query, parameters);
            return count;
        }

        public async Task<(bool Success, string Message)> AddBenefitToEmployeeAsync(int employeeId, int companyId, string benefitName, string benefitType, int? NumDependents = null, string? PensionType = null)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                var query = @"
                    INSERT INTO PlaniFy.BeneficioEmpleado (idEmpleado, NombreBeneficio, idEmpresa, TipoBeneficio, NumeroDependientes, TipoPension)
                    VALUES (@EmployeeId, @BenefitName, @CompanyId, @BenefitType, @NumDependents, @PensionType)";

                var parameters = new 
                { 
                    EmployeeId = employeeId, 
                    CompanyId = companyId, 
                    BenefitName = benefitName,
                    BenefitType = benefitType,
                    NumDependents = NumDependents,
                    PensionType = PensionType
                };

                await connection.ExecuteAsync(query, parameters);
                
                return (true, BENEFIT_ADDED_MESSAGE);
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, BENEFIT_ADD_ERROR);
                return (false, $"Error al agregar el beneficio: {ex.Message}");
            }
        }
        
        public async Task<EmployeeBenefitsSummaryDto> GetEmployeeBenefitsSummaryAsync(int employeeId, int companyId, BenefitFilterDto? filter = null)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var parameters = new DynamicParameters();
            parameters.Add("EmployeeId", employeeId);
            parameters.Add("CompanyId", companyId);
            parameters.Add("SearchTerm", filter?.SearchTerm);
            parameters.Add("CalculationType", filter?.CalculationType);
            parameters.Add("Status", filter?.Status);

            using var multi = await connection.QueryMultipleAsync("PlaniFy.GetEmployeeBenefitsSummary", parameters, commandType: System.Data.CommandType.StoredProcedure);
            
            var benefits = MapBenefitsFromStoredProcedure(await multi.ReadAsync());
            
            if (multi.IsConsumed)
            {
                _logger.LogWarning("Second result set is consumed");
                return new EmployeeBenefitsSummaryDto
                {
                    AvailableBenefits = benefits.Where(b => !b.IsSelected).ToList(),
                    SelectedBenefits = benefits.Where(b => b.IsSelected).ToList(),
                    CurrentSelections = 0,
                    MaxSelections = 0
                };
            }
            
            var summary = await multi.ReadFirstAsync<dynamic>();
            
            // Log summary data for debugging
            if (summary != null)
            {
                var summaryDict = (IDictionary<string, object>)summary;
                _logger.LogInformation("Summary columns: {Columns}", string.Join(", ", summaryDict.Keys));
            }

            return new EmployeeBenefitsSummaryDto
            {
                AvailableBenefits = benefits.Where(b => !b.IsSelected).ToList(),
                SelectedBenefits = benefits.Where(b => b.IsSelected).ToList(),
                CurrentSelections = GetSummaryValue<int>(summary, "CurrentSelections"),
                MaxSelections = GetSummaryValue<int>(summary, "MaxSelections")
            };
        }

        public async Task<int> GetMaxBenefitLimitAsync(int companyId)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var query = @"
                SELECT TOP 1 ISNULL(MaximoBeneficios, @DefaultMax)
                FROM PlaniFy.Empresa
                WHERE Id = @CompanyId";

            var parameters = new { CompanyId = companyId, DefaultMax = DEFAULT_MAX_BENEFITS };

            try
            {
                var result = await connection.QuerySingleOrDefaultAsync<int?>(query, parameters);
                return result ?? DEFAULT_MAX_BENEFITS;
            }
            catch
            {
                return DEFAULT_MAX_BENEFITS;
            }
        }

        public async Task<int> GetTotalEmployeeCountAsync(int companyId)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var query = @"
                SELECT COUNT(DISTINCT idPersona)
                FROM PlaniFy.Empleado
                WHERE IdEmpresa = @CompanyId";

            var parameters = new { CompanyId = companyId };

            var count = await connection.QuerySingleAsync<int>(query, parameters);
            return count;
        }

        private List<EmployeeBenefitDto> MapBenefitsFromStoredProcedure(IEnumerable<dynamic> result)
        {
            var benefits = new List<EmployeeBenefitDto>();
            
            foreach (var r in result)
            {
                try
                {
                    var rowDict = (IDictionary<string, object>)r;
                    
                    LogFirstRowColumns(rowDict, benefits.Count);
                    
                    EmployeeBenefitDto benefit = CreateBenefitFromRow(rowDict);
                    benefits.Add(benefit);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, STRING_NULL_MESSAGE);
                }
            }
            
            return benefits;
        }

        private void LogFirstRowColumns(IDictionary<string, object> rowDict, int count)
        {
            if (count == 0)
            {
                _logger.LogInformation("Available columns: {Columns}", string.Join(", ", rowDict.Keys));
            }
        }

        private EmployeeBenefitDto CreateBenefitFromRow(IDictionary<string, object> rowDict)
        {
            return new EmployeeBenefitDto
            {
                CompanyId = GetPropertyValue<int>(rowDict, "CompanyId"),
                BenefitName = GetPropertyValue<string>(rowDict, "BenefitName") ?? string.Empty,
                CalculationType = GetPropertyValue<string>(rowDict, "CalculationType") ?? string.Empty,
                BenefitType = GetPropertyValue<string>(rowDict, "BenefitType") ?? string.Empty,
                Value = GetPropertyValue<int?>(rowDict, "Value"),
                Percentage = GetPropertyValue<int?>(rowDict, "Percentage"),
                IsSelected = IsBenefitSelected(rowDict),
                EmployeeCount = GetPropertyValue<int>(rowDict, "EmployeeCount"),
                UsagePercentage = GetPropertyValue<double>(rowDict, "UsagePercentage"),
                // Employee-specific fields from BeneficioEmpleado table
                NumDependents = GetPropertyValue<int?>(rowDict, "NumDependents"),
                PensionType = GetPropertyValue<string>(rowDict, "PensionType")
            };
        }

        private T? GetPropertyValue<T>(IDictionary<string, object> rowDict, string key)
        {
            if (!rowDict.TryGetValue(key, out var value) || value == null)
            {
                return default(T);
            }
            
            try
            {
                var directValue = TryGetDirectValue<T>(value);
                if (directValue != null)
                {
                    return directValue;
                }
                
                var nestedValue = TryGetNestedValue<T>(value, key);
                if (nestedValue != null)
                {
                    return nestedValue;
                }
                
                var reflectionValue = TryGetReflectionValue<T>(value, key);
                if (reflectionValue != null)
                {
                    return reflectionValue;
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error extracting property value for key: {Key}", key);
            }
            
            return default(T);
        }

        private T? TryGetDirectValue<T>(object value)
        {
            if (value is T directValue)
            {
                return directValue;
            }
            
            if (value is IConvertible)
            {
                return (T)Convert.ChangeType(value, typeof(T));
            }
            
            return default(T);
        }

        private T? TryGetNestedValue<T>(object value, string key)
        {
            if (value is not IDictionary<string, object> nestedDict)
            {
                return default(T);
            }
            
            if (nestedDict.TryGetValue(key, out var nestedValue) && nestedValue is T)
            {
                return (T)nestedValue;
            }
            
            if (nestedDict.TryGetValue("Value", out var valueProp) && valueProp is T)
            {
                return (T)valueProp;
            }
            
            return GetFirstMatchingValue<T>(nestedDict);
        }

        private T? GetFirstMatchingValue<T>(IDictionary<string, object> nestedDict)
        {
            foreach (var kvp in nestedDict)
            {
                if (kvp.Value is T)
                {
                    return (T)kvp.Value;
                }
            }
            
            return default(T);
        }

        private T? TryGetReflectionValue<T>(object value, string key)
        {
            Type type = value.GetType();
            
            PropertyInfo? property = type.GetProperty(key);
            if (property != null)
            {
                object? propValue = property.GetValue(value);
                if (propValue is T)
                {
                    return (T)propValue;
                }
            }
            
            PropertyInfo? valueProp = type.GetProperty("Value");
            if (valueProp != null)
            {
                object? propValue = valueProp.GetValue(value);
                if (propValue is T)
                {
                    return (T)propValue;
                }
            }
            
            return default(T);
        }

        private bool IsBenefitSelected(IDictionary<string, object> rowDict)
        {
            if (rowDict.TryGetValue("IsSelected", out var value) && value != null)
            {
                if (value is int intVal)
                    return intVal == 1;
                if (value is bool boolVal)
                    return boolVal;
            }
            return false;
        }

        private T GetSummaryValue<T>(dynamic summary, string key)
        {
            try
            {
                if (summary == null)
                    return default(T);
                
                var summaryDict = (IDictionary<string, object>)summary;
                if (summaryDict.TryGetValue(key, out var value) && value != null)
                {
                    return (T)Convert.ChangeType(value, typeof(T));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting summary value for key: {Key}", key);
            }
            
            return default(T);
        }
    }
}

