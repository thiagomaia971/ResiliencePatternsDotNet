using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ResiliencePatterns.DotNet.Domain.Commands;

namespace ResiliencePatterns.DotNet.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ExecuteController : BaseController
    {
        [HttpPost]
        public async Task<IActionResult> Execute(ExecuteCommand executeCommand) 
            => HandleResult(Mediator.Send(executeCommand).GetAwaiter().GetResult());
    }
}