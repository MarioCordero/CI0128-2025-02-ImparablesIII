// TEST
using Microsoft.Data.SqlClient;
using Dapper;
using backend.Models;

namespace backend.Repositories
{
    public class OrderRepository
    {
        private readonly string _connectionString;

        public OrderRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("CountryContext");
        }

        public async Task<IEnumerable<OrderDetailDto>> GetOrderDetailsAsync(int orderId)
        {
            using var connection = new SqlConnection(_connectionString);
            var sql = @"
                SELECT o.id AS OrderID,
                       o.SubmitDate,
                       o.CustomerId,
                       o.Status,
                       p.Name AS Name,
                       op.Quantity,
                       p.UnitPrice,
                       (op.Quantity * p.UnitPrice) AS Subtotal
                FROM MarioCordero.Orders o
                JOIN MarioCordero.OrdersProducts op ON o.id = op.OrderId
                JOIN MarioCordero.Products p ON op.ProductId = p.id
                WHERE o.id = @OrderId;
            ";
            return await connection.QueryAsync<OrderDetailDto>(sql, new { OrderId = orderId });
        }
    }
}