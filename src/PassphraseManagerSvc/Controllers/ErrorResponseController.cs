using System.Net;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;
using PassphraseManagerSvc.Dto;
using System.Threading.Tasks;

namespace PassphraseManagerSvc.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class ErrorResponseController : ControllerBase
    {
        private readonly ILogger<ErrorResponseController> _logger;

        public ErrorResponseController(ILogger<ErrorResponseController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        [HttpPost]
        [HttpPut]
        [HttpDelete]
        public ApiResponeDto<string> Get()
        {
            var feature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();
            if (feature?.Error is PassStoreException)
            {
                var ex = feature?.Error as PassStoreException;
                return new ApiResponeDto<string>()
                {
                    Result = string.Empty,
                    HasError = true,
                    ErrorDetails = new Error()
                    {
                        ErrorCode = ex.Category.ToString(),
                        Message = ex.Message
                    }
                };
            }

            return
            new ApiResponeDto<string>()
            {
                Result = null,
                HasError = true,
                ErrorDetails = new Error() { ErrorCode = HttpStatusCode.InternalServerError.ToString(), Message = feature?.Error.Message }
            };

        }

    }
}