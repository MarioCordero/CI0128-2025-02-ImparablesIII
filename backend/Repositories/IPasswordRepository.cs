namespace backend.Repositories
{
    public interface IPasswordRepository
    {
        Task<bool> UpdateEmployeePasswordAsync(int personaId, string password);
        Task<bool> TestConnectionAsync();
    }
}
