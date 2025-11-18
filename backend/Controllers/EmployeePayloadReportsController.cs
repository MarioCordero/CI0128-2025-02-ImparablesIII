using Microsoft.AspNetCore.Mvc;
using backend.Services;
using backend.DTOs;
using backend.Constants;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/employees")]
    public class EmployeePayrollReportsController : ControllerBase
    {
        private readonly IPayrollService _payrollService;
        private readonly IReportGenerationService _reportGenerationService;
        private readonly ILogger<EmployeePayrollReportsController> _logger;

        public EmployeePayrollReportsController(
          IPayrollService payrollService,
          IReportGenerationService reportGenerationService,
          ILogger<EmployeePayrollReportsController> logger)
        {
          _payrollService = payrollService;
          _reportGenerationService = reportGenerationService;
          _logger = logger;
        }

        [HttpGet("{id:int}/payroll-reports")]
        public async Task<ActionResult<List<EmployeePayrollReportDto>>> GetEmployeePayrollReports(
          int id,
          [FromQuery] int authenticatedEmployeeId,
          [FromQuery] int? year = null,
          [FromQuery] int? month = null,
          [FromQuery] string? puesto = null)
        {
            try
            {
              _logger.LogInformation("Solicitud de reportes de planilla para empleado {EmployeeId}", id);

              if (id <= 0)
              {
                return BadRequest(new { message = ReturnMessagesConstants.Validation.EmployeeIdMustBeGreaterThanZero });
              }

              if (authenticatedEmployeeId <= 0)
              {
                return BadRequest(new { message = ReturnMessagesConstants.Validation.AuthenticatedEmployeeIdRequired });
              }

              var reports = await _payrollService.GetEmployeePayrollReportsAsync(
                id, 
                authenticatedEmployeeId, 
                year, 
                month, 
                puesto);

              _logger.LogInformation("Reportes enviados exitosamente para empleado {EmployeeId}. Total: {Count}", id, reports.Count);
              return Ok(reports);
            }
            catch (UnauthorizedAccessException ex)
            {
              _logger.LogWarning("Acceso no autorizado para empleado {EmployeeId}: {Message}", id, ex.Message);
              return Unauthorized(new { message = ReturnMessagesConstants.General.UnauthorizedAccess });
            }
            catch (ArgumentOutOfRangeException ex)
            {
              _logger.LogWarning("Argumento inválido para empleado {EmployeeId}: {Message}", id, ex.Message);
              return BadRequest(new { message = ReturnMessagesConstants.Validation.InvalidArgument });
            }
            catch (Exception ex)
            {
              _logger.LogError(ex, "Error interno obteniendo reportes de planilla para empleado {EmployeeId}", id);
              return StatusCode(500, new 
              { 
                  message = ReturnMessagesConstants.General.InternalServerError,
                  detail = ex.Message 
              });
            }
        }

        [HttpGet("{id:int}/payroll-reports/{payrollId:int}/detailed")]
        public async Task<ActionResult<DetailedPayrollReportDto>> GetDetailedPayrollReport(
            int id,
            int payrollId,
            [FromQuery] int authenticatedEmployeeId)
        {
            try
            {
                _logger.LogInformation("Solicitud de reporte detallado de planilla para empleado {EmployeeId}, planilla {PayrollId}", id, payrollId);

                if (id <= 0)
                {
                    return BadRequest(new { message = ReturnMessagesConstants.Validation.EmployeeIdMustBeGreaterThanZero });
                }

                if (authenticatedEmployeeId <= 0)
                {
                    return BadRequest(new { message = ReturnMessagesConstants.Validation.AuthenticatedEmployeeIdRequired });
                }

                var report = await _payrollService.GetDetailedPayrollReportAsync(id, payrollId, authenticatedEmployeeId);

                if (report == null)
                {
                    return NotFound(new { message = ReturnMessagesConstants.Payroll.ReportNotFound });
                }

                return Ok(report);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning("Acceso no autorizado para empleado {EmployeeId}: {Message}", id, ex.Message);
                return Unauthorized(new { message = ReturnMessagesConstants.General.UnauthorizedAccess });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error interno obteniendo reporte detallado para empleado {EmployeeId}", id);
                return StatusCode(500, new 
                { 
                    message = ReturnMessagesConstants.General.InternalServerError,
                    detail = ex.Message 
                });
            }
        }

        [HttpGet("{id:int}/payroll-reports/{payrollId:int}/download/pdf")]
        public async Task<IActionResult> DownloadPdfReport(
            int id,
            int payrollId,
            [FromQuery] int authenticatedEmployeeId)
        {
            try
            {
                _logger.LogInformation("Descarga de reporte PDF para empleado {EmployeeId}, planilla {PayrollId}", id, payrollId);

                var report = await _payrollService.GetDetailedPayrollReportAsync(id, payrollId, authenticatedEmployeeId);

                if (report == null)
                {
                    return NotFound(new { message = ReturnMessagesConstants.Payroll.ReportNotFound });
                }

                var pdfBytes = _reportGenerationService.GeneratePdfReport(report);
                var fileName = $"Reporte_Planilla_{report.FechaGeneracion:yyyyMMdd}.pdf";

                return File(pdfBytes, "application/pdf", fileName);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning("Acceso no autorizado para empleado {EmployeeId}: {Message}", id, ex.Message);
                return Unauthorized(new { message = ReturnMessagesConstants.General.UnauthorizedAccess });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generando reporte PDF para empleado {EmployeeId}", id);
                return StatusCode(500, new 
                { 
                    message = ReturnMessagesConstants.Payroll.ErrorGeneratingPdfReport,
                    detail = ex.Message 
                });
            }
        }
    }
}