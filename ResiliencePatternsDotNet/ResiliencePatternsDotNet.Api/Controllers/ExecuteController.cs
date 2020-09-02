using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ResiliencePatternsDotNet.Commons.Services;

namespace ResiliencePatternsDotNet.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ExecuteController : ControllerBase
    {
        private readonly ILogger<ExecuteController> _logger;
        private readonly IExecuteService _executeService;

        public ExecuteController(ILogger<ExecuteController> logger, IExecuteService executeService)
        {
            _logger = logger;
            _executeService = executeService;
        }

        [HttpPost]
        public void Post()
        {
            _logger.LogInformation("Chegou");
            _executeService.Execute();
        }
    }
}