using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using backend.Services;
using backend.Repositories;
using backend.DTOs;
using backend.Constants;
using backend.Services.PaymentsCalculate;

namespace backend.Tests
{
    [TestClass]
    public class PayrollServiceTests
    {
        private Mock<IPayrollRepository> _repoMock = null!;
        private Mock<IBenefitDeductionsService> _benefitsMock = null!;
        private Mock<IEmployeeBenefitRepository> _employeeBenefitRepoMock = null!;
        private EmployeeDeductionCalculatorFactory _employeeCalculatorFactory = null!;
        private EmployerDeductionCalculatorFactory _employerCalculatorFactory = null!;
        private IBenefitCodeParser _benefitCodeParser = null!;
        private PayrollService _service = null!;

        [TestInitialize]
        public void Setup()
        {
            _repoMock = new Mock<IPayrollRepository>(MockBehavior.Strict);
            _benefitsMock = new Mock<IBenefitDeductionsService>(MockBehavior.Strict);
            _employeeBenefitRepoMock = new Mock<IEmployeeBenefitRepository>(MockBehavior.Strict);
            _employeeCalculatorFactory = new EmployeeDeductionCalculatorFactory();
            _employerCalculatorFactory = new EmployerDeductionCalculatorFactory();
            _benefitCodeParser = new BenefitCodeParser();
            _service = new PayrollService(
                _repoMock.Object, 
                _benefitsMock.Object, 
                _employeeBenefitRepoMock.Object,
                _employeeCalculatorFactory,
                _employerCalculatorFactory,
                _benefitCodeParser);
        }

        [TestMethod]
        public async Task GetEmployeePayrollWithDeductionsAsync_ComputesEmployeeDeductionsAndNet()
        {
            var companyId = 1;
            var employees = new List<EmployeePayrollDto>
            {
                new EmployeePayrollDto { IdEmpleado = 10, IdEmpresa = companyId, Nombre = "A", Apellidos = "A", SalarioBruto = 100000m },
            };
            var deductions = new List<EmployeeDeductionDto>
            {
                new EmployeeDeductionDto { Code = "CCSS_SEM_EE", Rate = 9.34m },
                new EmployeeDeductionDto { Code = "CCSS_IVM_EE", Rate = 3.84m },
                new EmployeeDeductionDto { Code = "BP_TRAB", Rate = 1.0m },
            };

            _repoMock.Setup(r => r.GetEmployeesForPayrollAsync(companyId)).ReturnsAsync(employees);
            _repoMock.Setup(r => r.GetEmployeeDeductionsAsync()).ReturnsAsync(deductions);

            var result = await _service.GetEmployeePayrollWithDeductionsAsync(companyId);

            Assert.AreEqual(1, result.Count);
            var emp = result[0];
            Assert.AreEqual(10, emp.IdEmpleado);
            Assert.IsTrue(emp.TotalEmployeeDeductions > 0);
            Assert.AreEqual(Math.Round(emp.SalarioBruto - emp.TotalEmployeeDeductions, 2), emp.NetSalary);
            Assert.IsTrue(emp.DeductionLines.Count >= 1);

            _repoMock.Verify(r => r.GetEmployeesForPayrollAsync(companyId), Times.Once);
            _repoMock.Verify(r => r.GetEmployeeDeductionsAsync(), Times.Once);
        }

        [TestMethod]
        public async Task GetEmployerPayrollWithDeductionsAsync_ComputesEmployerDeductions()
        {
            var companyId = 1;
            var employees = new List<EmployeePayrollDto>
            {
                new EmployeePayrollDto { IdEmpleado = 20, IdEmpresa = companyId, Nombre = "B", Apellidos = "B", SalarioBruto = 200000m },
            };
            var deductions = new List<EmployerDeductionDto>
            {
                new EmployerDeductionDto { Code = "CCSS_SEM_ER", Rate = 26.33m },
                new EmployerDeductionDto { Code = "CCSS_IVM_ER", Rate = 5.08m },
                new EmployerDeductionDto { Code = "BP_PATRON", Rate = 0.25m },
            };

            _repoMock.Setup(r => r.GetEmployeesForPayrollAsync(companyId)).ReturnsAsync(employees);
            _repoMock.Setup(r => r.GetEmployerDeductionsAsync()).ReturnsAsync(deductions);

            var result = await _service.GetEmployerPayrollWithDeductionsAsync(companyId);

            Assert.AreEqual(1, result.Count);
            var emp = result[0];
            Assert.AreEqual(20, emp.IdEmpleado);
            Assert.IsTrue(emp.TotalEmployerDeductions > 0);
            Assert.IsTrue(emp.DeductionLines.Count >= 1);

            _repoMock.Verify(r => r.GetEmployeesForPayrollAsync(companyId), Times.Once);
            _repoMock.Verify(r => r.GetEmployerDeductionsAsync(), Times.Once);
        }

        [TestMethod]
        public async Task GeneratePayrollWithBenefitsAsync_Monthly_ThrowsIfAlreadyExists()
        {
            var companyId = 2;
            _repoMock.Setup(r => r.ExistsPayrollForMonthAsync(companyId, It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(true);

            await Assert.ThrowsExceptionAsync<InvalidOperationException>(() => _service.GeneratePayrollWithBenefitsAsync(companyId, responsibleEmployeeId: 1, hours: 160));

            _repoMock.Verify(r => r.ExistsPayrollForMonthAsync(companyId, It.IsAny<int>(), It.IsAny<int>()), Times.Once);
        }

        [TestMethod]
        public async Task GeneratePayrollWithBenefitsAsync_Biweekly_ThrowsIfFortnightExists()
        {
            var companyId = 3;
            _repoMock.Setup(r => r.ExistsPayrollForMonthAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(false);
            _repoMock.Setup(r => r.ExistsPayrollForFortnightAsync(companyId, It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(true);

            await Assert.ThrowsExceptionAsync<InvalidOperationException>(() => _service.GeneratePayrollWithBenefitsAsync(companyId, 1, 80, periodType: "Quincenal", fortnight: 1));

            _repoMock.Verify(r => r.ExistsPayrollForFortnightAsync(companyId, It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()), Times.Once);
        }

        [TestMethod]
        public async Task GeneratePayrollWithBenefitsAsync_Succeeds_InsertsPayrollAndDetails()
        {
            var companyId = 5;
            var employees = new List<EmployeePayrollDto>
            {
                new EmployeePayrollDto { IdEmpleado = 100, IdEmpresa = companyId, Nombre = "C", Apellidos = "C", SalarioBruto = 150000m, TotalEmployeeDeductions = 0m },
                new EmployeePayrollDto { IdEmpleado = 101, IdEmpresa = companyId, Nombre = "D", Apellidos = "D", SalarioBruto = 120000m, TotalEmployeeDeductions = 0m },
            };
            var employerResults = new List<EmployerDeductionResultDto>
            {
                new EmployerDeductionResultDto { IdEmpleado = 100, IdEmpresa = companyId, Nombre = "C", Apellidos = "C", SalarioBruto = 150000m, TotalEmployerDeductions = 10000m },
                new EmployerDeductionResultDto { IdEmpleado = 101, IdEmpresa = companyId, Nombre = "D", Apellidos = "D", SalarioBruto = 120000m, TotalEmployerDeductions = 8000m },
            };

            var emp100Benefits = new BenefitDeductionCalculationDto
            {
                Deductions = new List<BenefitDeductionItemDto>
                {
                    new BenefitDeductionItemDto { Code = "BEN1", Amount = 2000, Role = DeductionRoleNames.EmployeeDeduction },
                    new BenefitDeductionItemDto { Code = "BEN2", Amount = 3000, Role = DeductionRoleNames.EmployerDeduction },
                }
            };
            var emp101Benefits = new BenefitDeductionCalculationDto
            {
                Deductions = new List<BenefitDeductionItemDto>
                {
                    new BenefitDeductionItemDto { Code = "BEN3", Amount = 1000, Role = DeductionRoleNames.EmployeeDeduction },
                }
            };

            _repoMock.Setup(r => r.ExistsPayrollForMonthAsync(companyId, It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(false);
            _repoMock.Setup(r => r.GetEmployeesForPayrollAsync(companyId)).ReturnsAsync(employees);
            _repoMock.Setup(r => r.GetEmployeeDeductionsAsync()).ReturnsAsync(new List<EmployeeDeductionDto>());
            // The service internally calls GetEmployerPayrollWithDeductionsAsync, which uses repo.GetEmployeesForPayrollAsync and GetEmployerDeductionsAsync.
            // We shortcut by stubbing GetEmployerDeductionsAsync and rely on the service to compute employerResults.
            _repoMock.Setup(r => r.GetEmployerDeductionsAsync()).ReturnsAsync(new List<EmployerDeductionDto>());
            _repoMock.Setup(r => r.GetEmployeePositionsByCompanyAsync(companyId)).ReturnsAsync(new Dictionary<int, string>());

            // However, the service maps employer results by IdEmpleado; to make it deterministic, we will bypass by stubbing the second method used inside service
            // by returning employees and zero employer deductions so TotalEmployerDeductions remains as provided in employerResults replacement path below.
            // Instead, we simulate employer results via a helper call path by intercepting after employee list: we can't replace internal list, so we align expectations accordingly
            // Use realistic path: set employer deductions empty; then TotalEmployerDeductions will be 0 and only benefits ER will count. We'll assert on called inserts accordingly.

            _benefitsMock.Setup(b => b.CalculateBenefitDeductionsAsync(100, companyId)).ReturnsAsync(emp100Benefits);
            _benefitsMock.Setup(b => b.CalculateBenefitDeductionsAsync(101, companyId)).ReturnsAsync(emp101Benefits);

            var insertedPayrollId = 777;
            _repoMock.Setup(r => r.InsertPayrollAsync(It.IsAny<PayrollInsertDto>())).ReturnsAsync(insertedPayrollId);

            List<PayrollDetailInsertDto>? capturedDetails = null;
            _repoMock
                .Setup(r => r.InsertPayrollDetailsAsync(insertedPayrollId, It.IsAny<List<PayrollDetailInsertDto>>()))
                .Callback<int, List<PayrollDetailInsertDto>>((id, details) => capturedDetails = details)
                .Returns(Task.CompletedTask);

            var resultId = await _service.GeneratePayrollWithBenefitsAsync(companyId, responsibleEmployeeId: 9, hours: 160);

            Assert.AreEqual(insertedPayrollId, resultId);
            Assert.IsNotNull(capturedDetails);
            Assert.AreEqual(2, capturedDetails!.Count);

            var d0 = capturedDetails[0];
            Assert.AreEqual(100, d0.IdEmpleado);
            // With no base employee/employer deductions, only benefits apply: EE=2000, ER=3000; net = 150000-2000
            Assert.AreEqual(150000, d0.SalarioBruto);
            Assert.AreEqual(2000, d0.DeduccionesEmpleado);
            Assert.AreEqual(3000, d0.DeduccionesEmpresa);
            Assert.AreEqual(5000, d0.TotalBeneficios);
            Assert.AreEqual(148000, d0.SalarioNeto);

            var d1 = capturedDetails[1];
            Assert.AreEqual(101, d1.IdEmpleado);
            Assert.AreEqual(120000, d1.SalarioBruto);
            Assert.AreEqual(1000, d1.DeduccionesEmpleado);
            Assert.AreEqual(0, d1.DeduccionesEmpresa);
            Assert.AreEqual(1000, d1.TotalBeneficios);
            Assert.AreEqual(119000, d1.SalarioNeto);

            _repoMock.Verify(r => r.ExistsPayrollForMonthAsync(companyId, It.IsAny<int>(), It.IsAny<int>()), Times.Once);
            _repoMock.Verify(r => r.GetEmployeesForPayrollAsync(companyId), Times.Exactly(2)); // once for employee path, once for employer path
            _repoMock.Verify(r => r.GetEmployerDeductionsAsync(), Times.Once);
            _repoMock.Verify(r => r.InsertPayrollAsync(It.IsAny<PayrollInsertDto>()), Times.Once);
            _repoMock.Verify(r => r.InsertPayrollDetailsAsync(insertedPayrollId, It.IsAny<List<PayrollDetailInsertDto>>()), Times.Once);
            _benefitsMock.Verify(b => b.CalculateBenefitDeductionsAsync(100, companyId), Times.Once);
            _benefitsMock.Verify(b => b.CalculateBenefitDeductionsAsync(101, companyId), Times.Once);
        }

        [TestMethod]
        public async Task GetLatestPayrollTotalsByCompanyAsync_ValidatesArgsAndDelegates()
        {
            var companyId = 10;
            var totals = new PayrollTotalsDto { TotalGross = 1, TotalNet = 2 };
            _repoMock.Setup(r => r.GetLatestPayrollTotalsByCompanyAsync(companyId)).ReturnsAsync(totals);

            var result = await _service.GetLatestPayrollTotalsByCompanyAsync(companyId);
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result!.TotalGross);
            Assert.AreEqual(2, result.TotalNet);

            await Assert.ThrowsExceptionAsync<ArgumentOutOfRangeException>(() => _service.GetLatestPayrollTotalsByCompanyAsync(0));

            _repoMock.Verify(r => r.GetLatestPayrollTotalsByCompanyAsync(companyId), Times.Once);
        }

        [TestMethod]
        public async Task GetPayrollHistoryByCompanyAsync_ValidatesArgsAndDelegates()
        {
            var companyId = 11;
            var history = new List<PayrollHistoryItemDto> { new PayrollHistoryItemDto { PayrollId = 1 } };
            _repoMock.Setup(r => r.GetPayrollHistoryByCompanyAsync(companyId)).ReturnsAsync(history);

            var result = await _service.GetPayrollHistoryByCompanyAsync(companyId);
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(1, result[0].PayrollId);

            await Assert.ThrowsExceptionAsync<ArgumentOutOfRangeException>(() => _service.GetPayrollHistoryByCompanyAsync(-1));

            _repoMock.Verify(r => r.GetPayrollHistoryByCompanyAsync(companyId), Times.Once);
        }
    }
}


