using backend.Models;
using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;

namespace backend.Repositories
{
	public class HoursRepository : IHoursRepository
	{
		private readonly string _connectionString;
        private const int DefaultRecentLimit = 6;

		public HoursRepository(IConfiguration configuration)
		{
			_connectionString = configuration.GetConnectionString("DefaultConnection") ?? "";
		}

		public async Task<Hours> RegisterHoursAsync(Hours entry)
		{
			const string query = @"
				INSERT INTO PlaniFy.HorasTrabajadas (idEmpleado, Cantidad, Detalle, Fecha, Estado, idAprobador)
				VALUES (@EmployeeId, @Quantity, @Detail, @Date, @Status, @ApproverId);
				SELECT CAST(SCOPE_IDENTITY() AS INT);
			";

			await using var connection = new SqlConnection(_connectionString);
			await connection.OpenAsync();

			var id = await connection.ExecuteScalarAsync<int>(query, new
			{
				entry.EmployeeId,
				entry.Quantity,
				entry.Detail,
				entry.Date,
				entry.Status,
				entry.ApproverId
			});

			entry.Id = id;
			return entry;
		}

		public async Task<List<Hours>> GetRecentEntriesAsync(int employeeId, int limit = DefaultRecentLimit)
		{
			const string query = @"
				SELECT TOP (@Limit)
                    id       AS Id,
                    idEmpleado AS EmployeeId,
                    Cantidad AS Quantity,
                    Detalle  AS Detail,
                    Fecha    AS Date,
                    Estado   AS Status,
                    idAprobador AS ApproverId
				FROM PlaniFy.HorasTrabajadas
				WHERE idEmpleado = @EmployeeId
				ORDER BY Fecha DESC, id DESC;
			";

			await using var connection = new SqlConnection(_connectionString);
			await connection.OpenAsync();

			var result = await connection.QueryAsync<Hours>(query, new { EmployeeId = employeeId, Limit = limit });
			return result.ToList();
		}

		public async Task<int> GetWeeklyHoursAsync(int employeeId)
		{
			const string query = @"
				SELECT COALESCE(SUM(Cantidad), 0)
				FROM PlaniFy.HorasTrabajadas
				WHERE idEmpleado = @EmployeeId
				AND Fecha >= DATEADD(DAY, -7, CAST(GETDATE() AS DATE));
			";

			await using var connection = new SqlConnection(_connectionString);
			await connection.OpenAsync();

			var total = await connection.ExecuteScalarAsync<int>(query, new { EmployeeId = employeeId });
			return total;
		}

		public async Task<int> GetMonthlyHoursAsync(int employeeId)
		{
			const string query = @"
				SELECT COALESCE(SUM(Cantidad), 0)
				FROM PlaniFy.HorasTrabajadas
				WHERE idEmpleado = @EmployeeId
				  AND Fecha >= DATEADD(MONTH, -1, CAST(GETDATE() AS DATE));
			";

			await using var connection = new SqlConnection(_connectionString);
			await connection.OpenAsync();

			var total = await connection.ExecuteScalarAsync<int>(query, new { EmployeeId = employeeId });
			return total;
		}

		public async Task<int> GetTotalEntriesAsync(int employeeId)
		{
			const string query = @"
				SELECT COUNT(*)
				FROM PlaniFy.HorasTrabajadas
				WHERE idEmpleado = @EmployeeId;
			";

			await using var connection = new SqlConnection(_connectionString);
			await connection.OpenAsync();

			var total = await connection.ExecuteScalarAsync<int>(query, new { EmployeeId = employeeId });
			return total;
		}

		public async Task<Hours?> GetLastEntryAsync(int employeeId)
		{
			const string query = @"
				SELECT TOP 1
					id          AS Id,
					idEmpleado  AS EmployeeId,
					Cantidad    AS Quantity,
					Detalle     AS Detail,
					Fecha       AS Date,
					Estado      AS Status,
					idAprobador AS ApproverId
				FROM PlaniFy.HorasTrabajadas
				WHERE idEmpleado = @EmployeeId
				ORDER BY Fecha DESC, id DESC;
			";

			await using var connection = new SqlConnection(_connectionString);
			await connection.OpenAsync();

			var last = await connection.QueryFirstOrDefaultAsync<Hours>(query, new { EmployeeId = employeeId });
			return last;
		}
	}
}
