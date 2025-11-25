using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using backend.Services;
using backend.Repositories;
using backend.DTOs;
using backend.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace backend.Tests
{
    [TestClass]
    public class ProjectServiceTests
    {
        private Mock<IProjectRepository> _projectRepoMock = null!;
        private Mock<IDirectionRepository> _directionRepoMock = null!;
        private ProjectService _service = null!;

        [TestInitialize]
        public void Setup()
        {
            _projectRepoMock = new Mock<IProjectRepository>(MockBehavior.Strict);
            _directionRepoMock = new Mock<IDirectionRepository>(MockBehavior.Strict);
            _service = new ProjectService(_projectRepoMock.Object, _directionRepoMock.Object);
        }

        // CREATE PROJECT TESTS
        [TestMethod]
        public async Task CreateProjectAsync_ValidProject_CreatesSuccessfully()
        {
            // Arrange
            var createDto = new CreateProjectDto
            {
                Nombre = "Nueva Empresa SA",
                CedulaJuridica = 123456789,
                Email = "nueva@empresa.com",
                PeriodoPago = "Mensual",
                Telefono = 12345678,
                Provincia = "San José",
                Canton = "Central",
                Distrito = "Carmen",
                DireccionParticular = "Calle 123",
                MaximoBeneficios = 5
            };

            var expectedDirectionId = 1;
            var expectedProjectResponse = new ProjectResponseDTO 
            { 
                Id = 1, 
                Nombre = "Nueva Empresa SA",
                CedulaJuridica = 123456789
            };
            var expectedDirection = new DirectionDTO { Id = expectedDirectionId };

            _projectRepoMock.Setup(r => r.ExistsByNameAsync(createDto.Nombre))
                          .ReturnsAsync(false);
            _projectRepoMock.Setup(r => r.ExistsByEmailAsync(createDto.Email))
                          .ReturnsAsync(false);
            _projectRepoMock.Setup(r => r.ExistsByLegalIdAsync(createDto.CedulaJuridica.ToString()))
                          .ReturnsAsync(false);
            _projectRepoMock.Setup(r => r.CreateDireccionAsync(
                createDto.Provincia, createDto.Canton, createDto.Distrito, createDto.DireccionParticular
            )).ReturnsAsync(expectedDirectionId);
            _projectRepoMock.Setup(r => r.CreateAsync(It.IsAny<Project>()))
                          .ReturnsAsync(expectedProjectResponse);
            _directionRepoMock.Setup(r => r.GetDirectionByIdAsync(expectedDirectionId))
                            .ReturnsAsync(expectedDirection);

            // Act
            var result = await _service.CreateProjectAsync(createDto);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedProjectResponse.Id, result.Id);
            Assert.AreEqual("Nueva Empresa SA", result.Nombre);
        }

        [TestMethod]
        public async Task CreateProjectAsync_DuplicateName_ThrowsArgumentException()
        {
            // Arrange
            var createDto = new CreateProjectDto { 
                Nombre = "Empresa Existente", 
                CedulaJuridica = 123456789 
            };
            _projectRepoMock.Setup(r => r.ExistsByNameAsync(createDto.Nombre))
                          .ReturnsAsync(true);

            // Act & Assert
            await Assert.ThrowsExceptionAsync<ArgumentException>(
                () => _service.CreateProjectAsync(createDto));
        }

        // GET PROJECTS TESTS
        [TestMethod]
        public async Task GetAllProjectsAsync_ReturnsProjectList()
        {
            // Arrange
            var projects = new List<ProjectResponseDTO>
            {
                new ProjectResponseDTO { Id = 1, Nombre = "Empresa 1", CedulaJuridica = 111111111 },
                new ProjectResponseDTO { Id = 2, Nombre = "Empresa 2", CedulaJuridica = 222222222 }
            };

            _projectRepoMock.Setup(r => r.GetAllAsync())
                          .ReturnsAsync(projects);

            // Act
            var result = await _service.GetAllProjectsAsync();

            // Assert
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual("Empresa 1", result[0].Nombre);
        }

        [TestMethod]
        public async Task GetProjectByIdAsync_ExistingId_ReturnsProject()
        {
            // Arrange
            var projectId = 1;
            var expectedProject = new ProjectResponseDTO 
            { 
                Id = projectId, 
                Nombre = "Empresa Test",
                CedulaJuridica = 123456789
            };
            
            _projectRepoMock.Setup(r => r.GetProjectWithDireccionAsync(projectId))
                          .ReturnsAsync(expectedProject);

            // Act
            var result = await _service.GetProjectByIdAsync(projectId);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(projectId, result!.Id);
        }

        [TestMethod]
        public async Task GetProjectsByEmployerIdAsync_ReturnsEmployerProjects()
        {
            // Arrange
            var employerId = 1;
            var projects = new List<ProjectResponseDTO>
            {
                new ProjectResponseDTO { Id = 1, Nombre = "Empresa A", CedulaJuridica = 111111111 },
                new ProjectResponseDTO { Id = 2, Nombre = "Empresa B", CedulaJuridica = 222222222 }
            };

            _projectRepoMock.Setup(r => r.GetByEmployerIdAsync(employerId))
                          .ReturnsAsync(projects);

            // Act
            var result = await _service.GetProjectsByEmployerIdAsync(employerId);

            // Assert
            Assert.AreEqual(2, result.Count);
        }

        // UPDATE PROJECT TESTS
        [TestMethod]
        public async Task UpdateProjectAsync_ValidUpdate_ReturnsSuccess()
        {
            // Arrange
            var projectId = 1;
            var updateDto = new UpdateProjectDTO
            {
                Nombre = "Empresa Actualizada",
                Direccion = new DirectionDTO
                {
                    Id = 1,
                    Provincia = "San José",
                    Canton = "Central",
                    Distrito = "Carmen",
                    DireccionParticular = "Calle Actualizada"
                }
            };

            var existingProject = new ProjectResponseDTO { Id = projectId, IdDireccion = 1 };
            var updatedProject = new ProjectResponseDTO { Id = projectId, Nombre = "Empresa Actualizada", CedulaJuridica = 123456789 };

            _projectRepoMock.Setup(r => r.GetByIdAsync(projectId))
                          .ReturnsAsync(existingProject);
            _projectRepoMock.Setup(r => r.UpdateDireccionAsync(existingProject.IdDireccion, updateDto.Direccion))
                          .ReturnsAsync(true);
            _projectRepoMock.Setup(r => r.UpdateAsync(projectId, updateDto))
                          .ReturnsAsync(true);
            _projectRepoMock.Setup(r => r.GetProjectWithDireccionAsync(projectId))
                          .ReturnsAsync(updatedProject);

            // Act
            var result = await _service.UpdateProjectAsync(projectId, updateDto);

            // Assert
            Assert.IsTrue(result.Success);
            Assert.IsNotNull(result.Project);
            Assert.AreEqual("Empresa Actualizada", result.Project!.Nombre);
        }

        [TestMethod]
        public async Task UpdateProjectAsync_NonExistentProject_ReturnsError()
        {
            // Arrange
            var projectId = 999;
            var updateDto = new UpdateProjectDTO { Nombre = "Empresa Inexistente" };

            _projectRepoMock.Setup(r => r.GetByIdAsync(projectId))
                          .ReturnsAsync((ProjectResponseDTO?)null);

            // Act
            var result = await _service.UpdateProjectAsync(projectId, updateDto);

            // Assert
            Assert.IsFalse(result.Success);
            Assert.AreEqual("Empresa no encontrada.", result.ErrorMessage);
        }

        // DASHBOARD TESTS
        [TestMethod]
        public async Task GetProjectsForDashboardAsync_ReturnsDashboardData()
        {
            // Arrange
            var employerId = 1;
            var projects = new List<ProjectResponseDTO>
            {
                new ProjectResponseDTO { Id = 1, Nombre = "Empresa Dashboard", CedulaJuridica = 123456789 }
            };

            _projectRepoMock.Setup(r => r.GetProjectsForDashboardAsync(employerId))
                          .ReturnsAsync(projects);
            _projectRepoMock.Setup(r => r.CountActiveEmployeesAsync(1))
                          .ReturnsAsync(5);

            // Act
            var result = await _service.GetProjectsForDashboardAsync(employerId);

            // Assert
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("Empresa Dashboard", result[0].Nombre);
        }

        // STATUS MANAGEMENT TESTS
        [TestMethod]
        public async Task DeleteProjectAsync_ValidId_ReturnsTrue()
        {
            // Arrange
            var projectId = 1;
            _projectRepoMock.Setup(r => r.DeleteAsync(projectId))
                          .ReturnsAsync(true);

            // Act
            var result = await _service.DeleteProjectAsync(projectId);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task ActivateProjectAsync_ValidId_ReturnsTrue()
        {
            // Arrange
            var projectId = 1;
            _projectRepoMock.Setup(r => r.ActivateAsync(projectId))
                          .ReturnsAsync(true);

            // Act
            var result = await _service.ActivateProjectAsync(projectId);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task DeactivateProjectAsync_ValidId_ReturnsTrue()
        {
            // Arrange
            var projectId = 1;
            _projectRepoMock.Setup(r => r.DeactivateAsync(projectId))
                          .ReturnsAsync(true);

            // Act
            var result = await _service.DeactivateProjectAsync(projectId);

            // Assert
            Assert.IsTrue(result);
        }

        // VALIDATION METHODS TESTS
        [TestMethod]
        public async Task ExistsByLegalIdAsync_ExistingLegalId_ReturnsTrue()
        {
            // Arrange
            var legalId = "123456789";
            _projectRepoMock.Setup(r => r.ExistsByLegalIdAsync(legalId))
                          .ReturnsAsync(true);

            // Act
            var result = await _service.ExistsByLegalIdAsync(legalId);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task ExistsByEmailAsync_ExistingEmail_ReturnsTrue()
        {
            // Arrange
            var email = "test@empresa.com";
            _projectRepoMock.Setup(r => r.ExistsByEmailAsync(email))
                          .ReturnsAsync(true);

            // Act
            var result = await _service.ExistsByEmailAsync(email);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task ProjectExistsAsync_ExistingProject_ReturnsTrue()
        {
            // Arrange
            var projectId = 1;
            _projectRepoMock.Setup(r => r.ExistsAsync(projectId))
                          .ReturnsAsync(true);

            // Act
            var result = await _service.ProjectExistsAsync(projectId);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task GetActiveEmployeesCountAsync_ValidProject_ReturnsCount()
        {
            // Arrange
            var projectId = 1;
            var expectedCount = 10;
            _projectRepoMock.Setup(r => r.CountActiveEmployeesAsync(projectId))
                          .ReturnsAsync(expectedCount);

            // Act
            var result = await _service.GetActiveEmployeesCountAsync(projectId);

            // Assert
            Assert.AreEqual(expectedCount, result);
        }

        [TestMethod]
        public async Task GetProjectWithDireccionAsync_ExistingProject_ReturnsProject()
        {
            // Arrange
            var projectId = 1;
            var expectedProject = new ProjectResponseDTO { Id = projectId, Nombre = "Empresa con Dirección", CedulaJuridica = 123456789 };
            _projectRepoMock.Setup(r => r.GetProjectWithDireccionAsync(projectId))
                          .ReturnsAsync(expectedProject);

            // Act
            var result = await _service.GetProjectWithDireccionAsync(projectId);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(projectId, result!.Id);
        }

        // COMPATIBILITY METHOD TEST
        [TestMethod]
        public async Task CreateProjectAsync_WithEmployerId_CreatesSuccessfully()
        {
            // Arrange
            var createDto = new CreateProjectDto 
            { 
                Nombre = "Empresa con Empleador", 
                CedulaJuridica = 123456789 
            };
            var employerId = 1;
            var expectedProjectResponse = new ProjectResponseDTO { Id = 1, CedulaJuridica = 123456789 };

            _projectRepoMock.Setup(r => r.ExistsByNameAsync(createDto.Nombre))
                          .ReturnsAsync(false);
            _projectRepoMock.Setup(r => r.ExistsByEmailAsync(It.IsAny<string>()))
                          .ReturnsAsync(false);
            _projectRepoMock.Setup(r => r.ExistsByLegalIdAsync(createDto.CedulaJuridica.ToString()))
                          .ReturnsAsync(false);
            _projectRepoMock.Setup(r => r.CreateDireccionAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                          .ReturnsAsync(1);
            _projectRepoMock.Setup(r => r.CreateAsync(It.IsAny<Project>()))
                          .ReturnsAsync(expectedProjectResponse);
            _directionRepoMock.Setup(r => r.GetDirectionByIdAsync(It.IsAny<int>()))
                            .ReturnsAsync(new DirectionDTO());

            // Act
            var result = await _service.CreateProjectAsync(createDto, employerId);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Id);
        }
    }
}