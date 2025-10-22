using backend.DTOs;
using backend.Models;
using backend.Repositories;

namespace backend.Services
{
    public class BenefitService : IBenefitService
    {
        private readonly IBenefitRepository _benefitRepository;
        private readonly IProjectRepository _projectRepository;

        public BenefitService(IBenefitRepository benefitRepository, IProjectRepository projectRepository)
        {
            _benefitRepository = benefitRepository;
            _projectRepository = projectRepository;
        }

        public async Task<BenefitResponseDto> CreateBenefitAsync(CreateBenefitDto createBenefitDto)
        {
            // Validate that the company exists
            if (!await _projectRepository.ExistsByIdAsync(createBenefitDto.CompanyId))
            {
                throw new ArgumentException("La empresa especificada no existe");
            }

            // Validate unique constraint
            if (await _benefitRepository.ExistsAsync(createBenefitDto.CompanyId, createBenefitDto.Name))
            {
                throw new ArgumentException("Ya existe un beneficio con este nombre para esta empresa");
            }

            // Create benefit entity
            var benefit = new Benefit
            {
                CompanyId = createBenefitDto.CompanyId,
                Name = createBenefitDto.Name.Trim(),
                CalculationType = createBenefitDto.CalculationType.Trim(),
                Type = createBenefitDto.Type.Trim()
            };

            // Save to database
            await _benefitRepository.CreateAsync(benefit);

            // Get company name for response
            var company = await _projectRepository.GetByIdAsync(createBenefitDto.CompanyId);
            var companyName = company?.Nombre ?? "Empresa no encontrada";

            return new BenefitResponseDto
            {
                CompanyId = benefit.CompanyId,
                Name = benefit.Name,
                CalculationType = benefit.CalculationType,
                Type = benefit.Type,
                CompanyName = companyName
            };
        }

        public async Task<List<BenefitResponseDto>> GetAllBenefitsAsync()
        {
            var benefits = await _benefitRepository.GetAllAsync();
            var result = new List<BenefitResponseDto>();

            foreach (var benefit in benefits)
            {
                var company = await _projectRepository.GetByIdAsync(benefit.CompanyId);
                result.Add(new BenefitResponseDto
                {
                    CompanyId = benefit.CompanyId,
                    Name = benefit.Name,
                    CalculationType = benefit.CalculationType,
                    Type = benefit.Type,
                    CompanyName = company?.Nombre ?? "Empresa no encontrada"
                });
            }

            return result;
        }

        public async Task<List<BenefitResponseDto>> GetBenefitsByCompanyIdAsync(int companyId)
        {
            // Validate that the company exists
            if (!await _projectRepository.ExistsByIdAsync(companyId))
            {
                throw new ArgumentException("La empresa especificada no existe");
            }

            return await _benefitRepository.GetBenefitsWithCompanyNameAsync(companyId);
        }

        public async Task<BenefitResponseDto?> GetBenefitByIdAsync(int companyId, string name)
        {
            var benefit = await _benefitRepository.GetByIdAsync(companyId, name);
            if (benefit == null)
            {
                return null;
            }

            var company = await _projectRepository.GetByIdAsync(benefit.CompanyId);
            return new BenefitResponseDto
            {
                CompanyId = benefit.CompanyId,
                Name = benefit.Name,
                CalculationType = benefit.CalculationType,
                Type = benefit.Type,
                CompanyName = company?.Nombre ?? "Empresa no encontrada"
            };
        }

        public async Task<BenefitResponseDto> UpdateBenefitAsync(int companyId, string name, UpdateBenefitDto updateDto)
        {
            // Validate that the benefit exists
            if (!await _benefitRepository.ExistsAsync(companyId, name))
            {
                throw new ArgumentException("El beneficio especificado no existe");
            }

            // If the name is being changed, check for uniqueness
            if (name != updateDto.Name && await _benefitRepository.ExistsAsync(companyId, updateDto.Name))
            {
                throw new ArgumentException("Ya existe un beneficio con este nombre para esta empresa");
            }

            // Update the benefit
            var success = await _benefitRepository.UpdateAsync(companyId, name, updateDto);
            if (!success)
            {
                throw new Exception("Error al actualizar el beneficio");
            }

            // Get updated benefit with company name
            var benefit = await _benefitRepository.GetByIdAsync(companyId, updateDto.Name);
            var company = await _projectRepository.GetByIdAsync(companyId);

            return new BenefitResponseDto
            {
                CompanyId = benefit!.CompanyId,
                Name = benefit.Name,
                CalculationType = benefit.CalculationType,
                Type = benefit.Type,
                CompanyName = company?.Nombre ?? "Empresa no encontrada"
            };
        }

        public async Task<bool> DeleteBenefitAsync(int companyId, string name)
        {
            // Validate that the benefit exists
            if (!await _benefitRepository.ExistsAsync(companyId, name))
            {
                throw new ArgumentException("El beneficio especificado no existe");
            }

            return await _benefitRepository.DeleteAsync(companyId, name);
        }

        public async Task<bool> ExistsBenefitAsync(int companyId, string name)
        {
            return await _benefitRepository.ExistsAsync(companyId, name);
        }

        public async Task<int> CountBenefitsByCompanyIdAsync(int companyId)
        {
            return await _benefitRepository.CountByCompanyIdAsync(companyId);
        }
    }
}
