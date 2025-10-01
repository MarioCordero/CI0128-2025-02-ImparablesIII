using backend.Models;

namespace backend.Repositories
{
    public interface IEmpresaRepository
    {
      Task<IEnumerable<Empresa>> GetAllEmpresasAsync();
      Task<Empresa?> GetEmpresaByIdAsync(int id);
      Task<IEnumerable<Beneficio>> GetBeneficiosByEmpresaIdAsync(int empresaId);
    }
}