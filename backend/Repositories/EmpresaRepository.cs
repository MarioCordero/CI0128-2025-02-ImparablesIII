using backend.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace backend.Repositories
{
  public class EmpresaRepository : IEmpresaRepository
  {
    private readonly string _connectionString;

    public EmpresaRepository(IConfiguration configuration)
    {
      _connectionString = configuration.GetConnectionString("DefaultConnection") 
          ?? throw new ArgumentNullException(nameof(configuration));
    }

    public async Task<IEnumerable<Empresa>> GetAllEmpresasAsync()
    {
      var empresas = new List<Empresa>();

      using var connection = new SqlConnection(_connectionString);
      await connection.OpenAsync();

      var query = @"
        SELECT e.Id, e.Nombre, e.CedulaJuridica, e.Email, e.PeriodoPago, e.Telefono, e.idDireccion,
                d.Provincia, d.Canton, d.Distrito, d.DireccionParticular
        FROM PlaniFy.Empresa e
        LEFT JOIN PlaniFy.Direccion d ON e.idDireccion = d.id
        ORDER BY e.Nombre";

      using var command = new SqlCommand(query, connection);
      using var reader = await command.ExecuteReaderAsync();

      while (await reader.ReadAsync())
      {
        var empresa = new Empresa
        {
          Id = reader.GetInt32("Id"),
          Nombre = reader.GetString("Nombre"),
          CedulaJuridica = reader.GetInt32("CedulaJuridica"),
          Email = reader.GetString("Email"),
          PeriodoPago = reader.GetString("PeriodoPago"),
          Telefono = reader.IsDBNull("Telefono") ? null : reader.GetInt32("Telefono"),
          IdDireccion = reader.GetInt32("idDireccion"),
          Direccion = new Direccion
          {
            Id = reader.GetInt32("idDireccion"),
            Provincia = reader.GetString("Provincia"),
            Canton = reader.GetString("Canton"),
            Distrito = reader.GetString("Distrito"),
            DireccionParticular = reader.GetString("DireccionParticular")
          }
        };

        empresas.Add(empresa);
      }

      // Load benefits for each company
      foreach (var empresa in empresas)
      {
        empresa.Beneficios = (await GetBeneficiosByEmpresaIdAsync(empresa.Id)).ToList();
      }

      return empresas;
    }

    public async Task<Empresa?> GetEmpresaByIdAsync(int id)
    {
      using var connection = new SqlConnection(_connectionString);
      await connection.OpenAsync();

      var query = @"
        SELECT e.Id, e.Nombre, e.CedulaJuridica, e.Email, e.PeriodoPago, e.Telefono, e.idDireccion,
                d.Provincia, d.Canton, d.Distrito, d.DireccionParticular
        FROM PlaniFy.Empresa e
        LEFT JOIN PlaniFy.Direccion d ON e.idDireccion = d.id
        WHERE e.Id = @Id";

      using var command = new SqlCommand(query, connection);
      command.Parameters.AddWithValue("@Id", id);
      using var reader = await command.ExecuteReaderAsync();

      if (await reader.ReadAsync())
      {
          var empresa = new Empresa
          {
            Id = reader.GetInt32("Id"),
            Nombre = reader.GetString("Nombre"),
            CedulaJuridica = reader.GetInt32("CedulaJuridica"),
            Email = reader.GetString("Email"),
            PeriodoPago = reader.GetString("PeriodoPago"),
            Telefono = reader.IsDBNull("Telefono") ? null : reader.GetInt32("Telefono"),
            IdDireccion = reader.GetInt32("idDireccion"),
            Direccion = new Direccion
            {
              Id = reader.GetInt32("idDireccion"),
              Provincia = reader.GetString("Provincia"),
              Canton = reader.GetString("Canton"),
              Distrito = reader.GetString("Distrito"),
              DireccionParticular = reader.GetString("DireccionParticular")
            }
          };

          empresa.Beneficios = (await GetBeneficiosByEmpresaIdAsync(empresa.Id)).ToList();

          return empresa;
      }

      return null;
    }

    public async Task<IEnumerable<Beneficio>> GetBeneficiosByEmpresaIdAsync(int empresaId)
    {
      var beneficios = new List<Beneficio>();

      using var connection = new SqlConnection(_connectionString);
      await connection.OpenAsync();

      var query = @"
        SELECT idEmpresa, Nombre, TipoCalculo, Tipo
        FROM PlaniFy.Beneficio
        WHERE idEmpresa = @IdEmpresa
        ORDER BY Nombre";

      using var command = new SqlCommand(query, connection);
      command.Parameters.AddWithValue("@IdEmpresa", empresaId);
      using var reader = await command.ExecuteReaderAsync();

      while (await reader.ReadAsync())
      {
        var beneficio = new Beneficio
        {
          IdEmpresa = reader.GetInt32("idEmpresa"),
          Nombre = reader.GetString("Nombre"),
          TipoCalculo = reader.GetString("TipoCalculo"),
          Tipo = reader.GetString("Tipo")
        };

        beneficios.Add(beneficio);
      }

      return beneficios;
    }
  }
}