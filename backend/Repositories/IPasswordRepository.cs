namespace backend.Repositories
{
    public interface IPasswordRepository
    {
        Task<bool> CreateUserAsync(int personaId, string password, string tipoUsuario = "Empleado");
        Task<bool> UpdateEmployeePasswordAsync(int personaId, string password);
        Task<bool> UserExistsAsync(int personaId);
        Task<bool> TestConnectionAsync();
    }
}
