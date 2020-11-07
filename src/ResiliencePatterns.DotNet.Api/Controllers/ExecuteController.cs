using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ResiliencePatterns.DotNet.Domain.Commands;
using ResiliencePatterns.DotNet.Domain.Services;
using ResiliencePatternsDotNet.Commons;

namespace ResiliencePatterns.DotNet.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ExecuteController : BaseController
    {
        private readonly IExecuteService _executeService;

        public ExecuteController(IExecuteService executeService)
        {
            _executeService = executeService;
        }
        
        [HttpPost]
        public MetricStatus Execute(ExecuteCommand executeCommand)
            => _executeService.Execute(executeCommand);
    }
}