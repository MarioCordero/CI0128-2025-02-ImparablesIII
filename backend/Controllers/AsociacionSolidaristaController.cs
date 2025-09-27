using Microsoft.AspNetCore.Mvc;
using backend.Models; // Usando modelos de la carpeta Models

namespace backend.Controllers
{
    /// <summary>
    /// Controlador para la API de Asociación Solidarista.
    /// Expone el endpoint para calcular los aportes del empleado y empleador según el salario bruto y la cédula de la empresa.
    /// </summary>
    [ApiController]
    [Route("api/asociacionsolidarista")]
    public class AsociacionSolidaristaController : ControllerBase
    {
        /// <summary>
        /// Calcula los aportes solidaristas del empleado y empleador.
        /// El cálculo depende del último dígito de la cédula de la empresa:
        /// - Si el último dígito está entre 0 y 4: 5% empleado, 5.33% empleador.
        /// - Si el último dígito está entre 5 y 9: 3.5% empleado, 3.83% empleador.
        /// </summary>
        /// <param name="cedulaEmpresa">Cédula de la empresa (debe terminar en un dígito).</param>
        /// <param name="salarioBruto">Salario bruto del empleado.</param>
        /// <param name="token">Token de autenticación requerido.</param>
        /// <returns>Un objeto JSON con las deducciones calculadas para empleado (EE) y empleador (ER).</returns>
        [HttpGet("aporte-empleado")]
        public ActionResult<AporteSolidaristaResponse> GetAporte([FromQuery] string cedulaEmpresa, [FromQuery] decimal salarioBruto, [FromQuery] string token)
        {
            if (string.IsNullOrEmpty(token)) return Forbid("Token is required");
            if (string.IsNullOrEmpty(cedulaEmpresa)) return BadRequest("cedulaEmpresa is required");
            if (salarioBruto <= 0) return BadRequest("salarioBruto must be positive");

            char ultimoDigito = cedulaEmpresa[^1];
            if (!char.IsDigit(ultimoDigito)) return BadRequest("cedulaEmpresa debe terminar en un dígito.");

            int digito = int.Parse(ultimoDigito.ToString());
            decimal porcentajeEmpleado, porcentajeEmpleador;

            if (digito >= 0 && digito <= 4)
            {
                porcentajeEmpleado = 0.05m;
                porcentajeEmpleador = 0.0533m;
            }
            else
            {
                porcentajeEmpleado = 0.035m;
                porcentajeEmpleador = 0.0383m;
            }

            // Uso del modelo AporteSolidaristaResponse para estructurar la respuesta
            var response = new AporteSolidaristaResponse
            {
                Deductions = new List<AporteSolidarista>
                {
                    new AporteSolidarista { Type = "EE", Amount = Math.Round(salarioBruto * porcentajeEmpleado, 2) },
                    new AporteSolidarista { Type = "ER", Amount = Math.Round(salarioBruto * porcentajeEmpleador, 2) }
                }
            };

            return Ok(response);
        }
    }
}