using Microsoft.AspNetCore.Mvc;
using ResiliencePatterns.DotNet.Domain.Commands;

namespace ResiliencePatterns.DotNet.Api.Controllers
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