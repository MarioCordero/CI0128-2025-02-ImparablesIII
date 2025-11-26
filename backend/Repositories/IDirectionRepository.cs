using backend.DTOs;
using backend.Models;

namespace backend.Repositories
{
    public interface IDirectionRepository
    {
        Task<int> CreateDireccionAsync(string provincia, string canton, string distrito, string? direccionParticular);
        Task<DirectionDTO?> GetDirectionByIdAsync(int id);
        Task<bool> UpdateDireccionAsync(int id, DirectionDTO direccion);
    }
}