using System;
using Microsoft.AspNetCore.Mvc;

namespace WebApplication.Controllers
{
    [ApiController]
    [Route("api/execute")]
    public class ExecuteController : ControllerBase
    {
        [HttpGet]
        public bool Executer()
        {
            Console.WriteLine("asidaiusdh");
            //Logger.LogInformation("Teste");
            return true;
        }
    }
}