using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace ResiliencePatterns.DotNet.Api.Controllers
{
    [ApiController]
    [Route("api/core/[controller]")]
    public class HalthCheckController : BaseController
    {
        // [SwaggerOperation(
        //     Summary = "Verifica se a API está ATIVA",
        //     Description = "Não precisa estar logado")]
        [Route("")]
        [HttpGet]
        public async Task<bool> Get()
        {
            return true;
        }
    }
}