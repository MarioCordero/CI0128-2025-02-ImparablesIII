using backend.DTOs;
using backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BenefitController : ControllerBase
    {
        private readonly IBenefitService _benefitService;
        private readonly ILogger<BenefitController> _logger;

        public BenefitController(IBenefitService benefitService, ILogger<BenefitController> logger)
        {
            _benefitService = benefitService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<List<BenefitResponseDto>>> GetAll()
        {
            try
            {
                var benefits = await _benefitService.GetAllBenefitsAsync();
                return Ok(benefits);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all benefits");
                return StatusCode(500, new { message = "Error retrieving benefits", error = ex.Message });
            }
        }

        [HttpGet("company/{companyId}")]
        public async Task<ActionResult<List<BenefitResponseDto>>> GetByCompanyId(int companyId)
        {
            try
            {
                var benefits = await _benefitService.GetBenefitsByCompanyIdAsync(companyId);
                return Ok(benefits);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid company ID: {CompanyId}", companyId);
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving benefits for company {CompanyId}", companyId);
                return StatusCode(500, new { message = "Error retrieving benefits", error = ex.Message });
            }
        }

        [HttpGet("company/{companyId}/benefit/{name}")]
        public async Task<ActionResult<BenefitResponseDto>> GetById(int companyId, string name)
        {
            try
            {
                var benefit = await _benefitService.GetBenefitByIdAsync(companyId, name);
                if (benefit == null)
                {
                    return NotFound(new { message = "Beneficio no encontrado" });
                }
                return Ok(benefit);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving benefit {Name} for company {CompanyId}", name, companyId);
                return StatusCode(500, new { message = "Error retrieving benefit", error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<ActionResult<BenefitResponseDto>> Create([FromBody] CreateBenefitDto createBenefitDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var benefit = await _benefitService.CreateBenefitAsync(createBenefitDto);
                return CreatedAtAction(nameof(GetById), 
                    new { companyId = benefit.CompanyId, name = benefit.Name }, 
                    benefit);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid data for benefit creation");
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating benefit");
                return StatusCode(500, new { message = "Error creating benefit", error = ex.Message });
            }
        }

        [HttpGet("company/{companyId}/benefit/{name}/exists")]
        public async Task<ActionResult<bool>> Exists(int companyId, string name)
        {
            try
            {
                var exists = await _benefitService.ExistsBenefitAsync(companyId, name);
                return Ok(exists);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if benefit exists: {CompanyId}, {Name}", companyId, name);
                return StatusCode(500, new { message = "Error checking benefit existence", error = ex.Message });
            }
        }
    }

}
