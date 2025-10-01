using Microsoft.AspNetCore.Mvc;
using backend.Models;

namespace backend.Controllers
{
    /// <summary>
    /// Controller for the Solidarist Association API.
    /// Exposes endpoints to calculate employee and employer contributions based on gross salary and company identification number.
    /// </summary>
    [ApiController]
    [Route("api/asociacionsolidarista")]
    public class AsociacionSolidaristaController : ControllerBase
    {
        /// <summary>
        /// Calculates solidarist contributions for both employee and employer.
        /// The authentication token must be sent in the Authorization header as "Bearer {token}".
        /// </summary>
        /// <param name="cedulaEmpresa">Company identification number (must end with a digit).</param>
        /// <param name="salarioBruto">Employee's gross salary.</param>
        /// <returns>A JSON object containing calculated deductions for employee (EE) and employer (ER).</returns>
        /// <response code="200">Returns the calculated contributions successfully.</response>
        /// <response code="400">Invalid parameters provided.</response>
        /// <response code="401">Missing or invalid authentication token.</response>
        [HttpGet("aporte-empleado")]
        public ActionResult<AporteSolidaristaResponse> GetAporte([FromQuery] string cedulaEmpresa, [FromQuery] decimal salarioBruto)
        {
            // Extract and validate Bearer token from Authorization header
            string? authHeader = Request.Headers["Authorization"];
            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
            {
                return Unauthorized("Token Bearer is required in Authorization header");
            }

            // Extract the actual token value (remove "Bearer " prefix)
            string token = authHeader.Substring("Bearer ".Length).Trim();
            if (string.IsNullOrEmpty(token))
            {
                return Unauthorized("Invalid token format");
            }

            // Validate input parameters
            if (string.IsNullOrEmpty(cedulaEmpresa)) 
                return BadRequest("cedulaEmpresa is required");
            
            if (salarioBruto <= 0) 
                return BadRequest("salarioBruto must be positive");

            // Extract the last digit from company ID for contribution rate calculation
            char ultimoDigito = cedulaEmpresa[^1];
            if (!char.IsDigit(ultimoDigito)) 
                return BadRequest("cedulaEmpresa debe terminar en un dígito.");

            // Convert last digit to integer for rate determination
            int digito = int.Parse(ultimoDigito.ToString());
            decimal porcentajeEmpleado, porcentajeEmpleador;

            // Determine contribution rates based on company ID's last digit
            // Companies ending in 0-4: Higher rates (5% employee, 5.33% employer)
            // Companies ending in 5-9: Lower rates (3.5% employee, 3.83% employer)
            if (digito >= 0 && digito <= 4)
            {
                porcentajeEmpleado = 0.05m;    // 5% employee contribution
                porcentajeEmpleador = 0.0533m; // 5.33% employer contribution
            }
            else
            {
                porcentajeEmpleado = 0.035m;   // 3.5% employee contribution
                porcentajeEmpleador = 0.0383m; // 3.83% employer contribution
            }

            // Calculate and build response with rounded amounts
            var response = new AporteSolidaristaResponse
            {
                Deductions = new List<AporteSolidarista>
                {
                    // EE = Employee (Empleado) contribution
                    new AporteSolidarista { Type = "EE", Amount = Math.Round(salarioBruto * porcentajeEmpleado, 2) },
                    // ER = Employer (Empleador) contribution
                    new AporteSolidarista { Type = "ER", Amount = Math.Round(salarioBruto * porcentajeEmpleador, 2) }
                }
            };

            return Ok(response);
        }
    }
}