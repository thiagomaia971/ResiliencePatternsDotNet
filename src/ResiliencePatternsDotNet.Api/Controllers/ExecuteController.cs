using Microsoft.AspNetCore.Mvc;
using ResiliencePatternsDotNet.Domain.Commands;

namespace ResiliencePatternsDotNet.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ExecuteController : BaseController
    {
        [HttpPost]
        public IActionResult Execute(ExecuteCommand executeCommand) 
            => HandleResult(Mediator.Send(executeCommand).GetAwaiter().GetResult());
    }
}