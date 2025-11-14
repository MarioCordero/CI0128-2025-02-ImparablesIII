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
            if (!await _projectRepository.ExistsByLegalIdAsync(createBenefitDto.CompanyId.ToString()))
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
                Percentage = createBenefitDto.Percentage,
                Descripcion = createBenefitDto.Descripcion
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
                Percentage = benefit.Percentage,
                Descripcion = benefit.Descripcion
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
                    Percentage = benefit.Percentage,
                    Descripcion = benefit.Descripcion
                });
            }

            return result;
        }

        public async Task<List<BenefitResponseDto>> GetBenefitsByCompanyIdAsync(int companyId)
        {
            // Validate that the company exists
            if (!await _projectRepository.ExistsAsync(companyId))
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
                Percentage = benefit.Percentage,
                Descripcion = benefit.Descripcion
            };
        }

        public async Task<bool> ExistsBenefitAsync(int companyId, string name)
        {
            return await _benefitRepository.ExistsAsync(companyId, name);
        }

        public async Task<UpdateBenefitResponseDto> UpdateBenefitAsync(int companyId, string name, UpdateBenefitRequestDto updateDto)
        {
            try
            {
                Console.WriteLine($"=== UPDATE VIA STORED PROCEDURE ===");

                // El stored procedure maneja todas las validaciones
                var updated = await _benefitRepository.UpdateAsync(companyId, name, updateDto);
                
                if (!updated)
                {
                    return new UpdateBenefitResponseDto
                    {
                        Success = false,
                        Message = "Error al actualizar el beneficio"
                    };
                }

                // Obtener el beneficio actualizado
                var updatedBenefit = await _benefitRepository.GetByIdAsync(companyId, updateDto.Name.Trim());
                
                if (updatedBenefit == null)
                {
                    return new UpdateBenefitResponseDto
                    {
                        Success = false,
                        Message = "No se pudo recuperar el beneficio actualizado"
                    };
                }

                var company = await _projectRepository.GetByIdAsync(companyId);

                return new UpdateBenefitResponseDto
                {
                    Success = true,
                    Message = "Beneficio actualizado correctamente",
                    UpdatedBenefit = new BenefitResponseDto
                    {
                        CompanyId = updatedBenefit.CompanyId,
                        Name = updatedBenefit.Name,
                        CalculationType = updatedBenefit.CalculationType,
                        Type = updatedBenefit.Type,
                        CompanyName = company?.Nombre ?? "Empresa no encontrada",
                        Value = updatedBenefit.Value,
                        Percentage = updatedBenefit.Percentage,
                        Descripcion = updatedBenefit.Descripcion
                    }
                };
            }
            catch (ArgumentException ex)
            {
                // Errores de validación del SP
                return new UpdateBenefitResponseDto
                {
                    Success = false,
                    Message = ex.Message
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in update: {ex.Message}");
                return new UpdateBenefitResponseDto
                {
                    Success = false,
                    Message = $"Error al actualizar: {ex.Message}"
                };
            }
        }
    }
}
