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

            var benefit = new Benefit
            {
                CompanyId = createBenefitDto.CompanyId,
                Name = createBenefitDto.Name.Trim(),
                CalculationType = createBenefitDto.CalculationType.Trim(),
                Type = createBenefitDto.Type.Trim(),
                Value = createBenefitDto.Value,
                Percentage = createBenefitDto.Percentage
            };
            

            // Save to database
            await _benefitRepository.CreateAsync(benefit);

            var company = await _projectRepository.GetByIdAsync(createBenefitDto.CompanyId);
            var companyName = company?.Nombre ?? "Empresa no encontrada";

            return new BenefitResponseDto
            {
                CompanyId = benefit.CompanyId,
                Name = benefit.Name,
                CalculationType = benefit.CalculationType,
                Type = benefit.Type,
                CompanyName = companyName,
                Value = benefit.Value,
                Percentage = benefit.Percentage
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
                    CompanyName = company?.Nombre ?? "Empresa no encontrada",
                    Value = benefit.Value,
                    Percentage = benefit.Percentage
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
                CompanyName = company?.Nombre ?? "Empresa no encontrada",
                Value = benefit.Value,
                Percentage = benefit.Percentage
            };
        }

        public async Task<bool> ExistsBenefitAsync(int companyId, string name)
        {
            return await _benefitRepository.ExistsAsync(companyId, name);
        }
    }
}
