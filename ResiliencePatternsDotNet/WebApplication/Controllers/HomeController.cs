using Microsoft.AspNetCore.Mvc;

namespace WebApplication.Controllers
{
    public class HomeController : ControllerBase
    {
        // GET
        public IActionResult Index()
        {
            return Ok("asd");
        }
    }
}