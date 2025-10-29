using backend.DTOs;
using backend.Models;

namespace backend.Repositories
{
    public interface IDireccionRepository
    {
        Task<int> CreateDireccionAsync(string provincia, string canton, string distrito, string? direccionParticular);
        Task<DireccionDto?> GetDireccionByIdAsync(int id);
        Task<bool> TestConnectionAsync();
        Task<bool> UpdateDireccionAsync(Direccion direccion);

    }
}