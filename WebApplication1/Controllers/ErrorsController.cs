using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using WebApplication1.ServiceDir;

namespace WebApplication1.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public class ErrorsController : ControllerBase
    {
        [Route("error")]
        public SvcOutput Error()
        {
            this.Response.StatusCode = StatusCodes.Status500InternalServerError; // You can use HttpStatusCode enum instead

            return new SvcOutput
            {
                Result = null,
                Message = "Произошла серверная ошибка. Программисту этого сервиса пора найти ошибку по логам.",
            };
        }
    }
}