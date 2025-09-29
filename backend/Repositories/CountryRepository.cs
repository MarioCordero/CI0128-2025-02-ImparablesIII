using backend_lab.Models;
using System.Data;
using System.Data.SqlClient;
using Dapper;
using Microsoft.Data.SqlClient;

namespace backend_lab.Repositories {
  public class CountryRepository {
    private readonly string _connectionString;
    public CountryRepository() {
      var builder = WebApplication.CreateBuilder();
      _connectionString = builder.Configuration.GetConnectionString("CountryContext");
    }
    public List<CountryModel> GetCountries() {
      using var connection = new SqlConnection(_connectionString);
      string query = "SELECT * FROM dbo.Country";
      return connection.Query<CountryModel>(query).ToList();
    }
  }
}