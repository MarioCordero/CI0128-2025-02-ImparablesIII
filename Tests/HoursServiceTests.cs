using System;
using backend.DTOs;
using backend.Models;
using backend.Repositories;
using backend.Services;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace backend.Tests
{
	[TestClass]
	public class HoursServiceTests
	{
		public TestContext TestContext { get; set; } = null!;

		private Mock<IHoursRepository> _hoursRepository = null!;
		private Mock<IEmployeeRepository> _employeeRepository = null!;
		private Mock<IProjectRepository> _projectRepository = null!;
		private Mock<ILogger<HoursService>> _logger = null!;
		private HoursService _hoursService = null!;

		[TestInitialize]
		public void Setup()
		{
			_hoursRepository = new Mock<IHoursRepository>();
			_employeeRepository = new Mock<IEmployeeRepository>();
			_projectRepository = new Mock<IProjectRepository>();
			_logger = new Mock<ILogger<HoursService>>();
			_hoursService = new HoursService(
				_hoursRepository.Object,
				_employeeRepository.Object,
				_projectRepository.Object,
				_logger.Object);
		}

		[TestCleanup]
		public void Cleanup()
		{
			var outcome = TestContext.CurrentTestOutcome;
			if (outcome == UnitTestOutcome.Passed || outcome == UnitTestOutcome.Failed)
			{
				Console.WriteLine($"{outcome}: {TestContext.TestName}");
			}
		}

		[TestMethod]
		public async Task RegisterHoursAsync_WithValidRequest_ReturnsSavedEntry()
		{
			// Arrange
			var request = new RegisterHoursRequestDto
			{
				EmployeeId = 5,
				Quantity = 8,
				Detail = "  Soporte ",
				Date = new DateTime(2025, 1, 15),
				ApproverId = 2,
				Status = "Aprobado"
			};

			_employeeRepository
				.Setup(repo => repo.GetEmployeeCompanyIdAsync(request.EmployeeId))
				.ReturnsAsync(15);

			_projectRepository
				.Setup(repo => repo.GetEmployerIdByCompanyIdAsync(15))
				.ReturnsAsync(3);

			_hoursRepository
				.Setup(repo => repo.RegisterHoursAsync(It.IsAny<Hours>()))
				.ReturnsAsync((Hours entry) =>
				{
					entry.Id = 99;
					return entry;
				});

			// Act
			var response = await _hoursService.RegisterHoursAsync(request);

			// Assert
			Assert.IsNotNull(response);
			Assert.AreEqual("Horas registradas correctamente.", response.Message);
			Assert.AreEqual(99, response.Entry.Id);
			Assert.AreEqual(request.EmployeeId, response.Entry.EmployeeId);
			Assert.AreEqual("Soporte", response.Entry.Detail);
			Assert.AreEqual(request.Quantity, response.Entry.Quantity);
			Assert.AreEqual(request.Status, response.Entry.Status);

			_hoursRepository.Verify(repo => repo.RegisterHoursAsync(It.Is<Hours>(h =>
				h.EmployeeId == request.EmployeeId &&
				h.Detail == "Soporte" &&
				h.Quantity == request.Quantity &&
				h.Status == request.Status &&
				h.ApproverId == 3
			)), Times.Once);
			_employeeRepository.Verify(repo => repo.GetEmployeeCompanyIdAsync(request.EmployeeId), Times.Once);
			_projectRepository.Verify(repo => repo.GetEmployerIdByCompanyIdAsync(15), Times.Once);
		}

		[TestMethod]
		public async Task RegisterHoursAsync_InvalidQuantity_ThrowsArgumentException()
		{
			// Arrange
			var request = new RegisterHoursRequestDto
			{
				EmployeeId = 1,
				Quantity = 0,
				Detail = "Test",
				ApproverId = 2
			};

			// Act & Assert
			await Assert.ThrowsExceptionAsync<ArgumentException>(() => _hoursService.RegisterHoursAsync(request));
		}

		[TestMethod]
		public async Task GetSummaryAsync_ReturnsAggregatedData()
		{
			// Arrange
			var employeeId = 10;
			var recentEntries = new List<Hours>
			{
				new Hours { Id = 1, EmployeeId = employeeId, Quantity = 4, Detail = "Reunión", Date = DateTime.UtcNow, Status = "Aprobado", ApproverId = 3 }
			};
			var lastEntry = new Hours { Id = 2, EmployeeId = employeeId, Quantity = 6, Detail = "Soporte", Date = DateTime.UtcNow, Status = "Pendiente", ApproverId = 4 };

			_hoursRepository.Setup(r => r.GetRecentEntriesAsync(employeeId, It.IsAny<int>()))
				.ReturnsAsync(recentEntries);
			_hoursRepository.Setup(r => r.GetWeeklyHoursAsync(employeeId)).ReturnsAsync(20);
			_hoursRepository.Setup(r => r.GetMonthlyHoursAsync(employeeId)).ReturnsAsync(80);
			_hoursRepository.Setup(r => r.GetTotalEntriesAsync(employeeId)).ReturnsAsync(12);
			_hoursRepository.Setup(r => r.GetLastEntryAsync(employeeId)).ReturnsAsync(lastEntry);

			// Act
			var summary = await _hoursService.GetSummaryAsync(employeeId);

			// Assert
			Assert.IsNotNull(summary);
			Assert.AreEqual(1, summary.RecentEntries.Count);
			Assert.AreEqual(20, summary.WeeklyHours);
			Assert.AreEqual(80, summary.MonthlyHours);
			Assert.AreEqual(12, summary.TotalEntries);
			Assert.IsNotNull(summary.LastEntry);
			Assert.AreEqual(lastEntry.Id, summary.LastEntry!.Id);
			Assert.AreEqual(lastEntry.Quantity, summary.LastEntry.Quantity);

			_hoursRepository.Verify(r => r.GetRecentEntriesAsync(employeeId, It.IsAny<int>()), Times.Once);
			_hoursRepository.Verify(r => r.GetWeeklyHoursAsync(employeeId), Times.Once);
			_hoursRepository.Verify(r => r.GetMonthlyHoursAsync(employeeId), Times.Once);
			_hoursRepository.Verify(r => r.GetTotalEntriesAsync(employeeId), Times.Once);
			_hoursRepository.Verify(r => r.GetLastEntryAsync(employeeId), Times.Once);
		}

		[TestMethod]
		public async Task GetLastEntryAsync_NoEntry_ReturnsNull()
		{
			var employeeId = 3;
			_hoursRepository.Setup(r => r.GetLastEntryAsync(employeeId))
				.ReturnsAsync((Hours?)null);

			var result = await _hoursService.GetLastEntryAsync(employeeId);

			Assert.IsNull(result);
			_hoursRepository.Verify(r => r.GetLastEntryAsync(employeeId), Times.Once);
		}

		[TestMethod]
		public async Task GetRecentEntriesAsync_InvalidEmployeeId_ThrowsArgumentException()
		{
			await Assert.ThrowsExceptionAsync<ArgumentException>(() => _hoursService.GetRecentEntriesAsync(0));
		}

		[TestMethod]
		public async Task GetRecentEntriesAsync_NegativeLimit_UsesDefault()
		{
			var employeeId = 7;
			_hoursRepository.Setup(r => r.GetRecentEntriesAsync(employeeId, It.IsAny<int>()))
				.ReturnsAsync(new List<Hours>());

			await _hoursService.GetRecentEntriesAsync(employeeId, -5);

			_hoursRepository.Verify(r => r.GetRecentEntriesAsync(employeeId, 6), Times.Once);
		}
	}
}