using Microsoft.AspNetCore.Mvc;
using backend.Repositories;
using backend.Models;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmpresaController : ControllerBase
    {
      private readonly IEmpresaRepository _empresaRepository;
      private readonly ILogger<EmpresaController> _logger;

      public EmpresaController(IEmpresaRepository empresaRepository, ILogger<EmpresaController> logger)
      {
        _empresaRepository = empresaRepository;
        _logger = logger;
      }

      [HttpGet]
      public async Task<IActionResult> GetAllEmpresas()
      {
        try
        {
          var empresas = await _empresaRepository.GetAllEmpresasAsync();
          return Ok(empresas);
        }
        catch (Exception ex)
        {
          _logger.LogError(ex, "Error retrieving all empresas");
          return StatusCode(500, new { message = "Error interno del servidor" });
        }
      }

      [HttpGet("{id}")]
      public async Task<IActionResult> GetEmpresaById(int id)
      {
        try
        {
          var empresa = await _empresaRepository.GetEmpresaByIdAsync(id);
          
          if (empresa == null)
          {
            return NotFound(new { message = "Empresa no encontrada" });
          }

          return Ok(empresa);
        }
        catch (Exception ex)
        {
          _logger.LogError(ex, "Error retrieving empresa with ID {Id}", id);
          return StatusCode(500, new { message = "Error interno del servidor: " + ex.Message });
        }
      }

      [HttpGet("{id}/beneficios")]
      public async Task<IActionResult> GetBeneficiosByEmpresaId(int id)
      {
        try
        {
          var beneficios = await _empresaRepository.GetBeneficiosByEmpresaIdAsync(id);
          return Ok(beneficios);
        }
        catch (Exception ex)
        {
          _logger.LogError(ex, "Error retrieving beneficios for empresa with ID {Id}", id);
          return StatusCode(500, new { message = "Error interno del servidor: " + ex.Message });
        }
      }
    }
}
