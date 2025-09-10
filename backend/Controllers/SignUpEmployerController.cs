using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace backend_lab_c28730.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SignUpEmployerController : ControllerBase
    {
        [HttpGet]
        public string Get()
        {
            return "SignUpEmployerController is working!";
        }
    }
}
