using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace LibreriaApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class ErrorController : ControllerBase
    {
        [Route("/error")]
        public ActionResult Error()
        {
            var context = HttpContext.Features.Get<IExceptionHandlerFeature>();
            var exception = context?.Error;

            var response = new
            {
                Message = "Ha ocurrido un error interno del servidor",
                Details = exception?.Message,
                Timestamp = DateTime.UtcNow
            };

            return StatusCode(500, response);
        }
    }
}
