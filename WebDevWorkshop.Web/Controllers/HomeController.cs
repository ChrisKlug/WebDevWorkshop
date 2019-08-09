using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebDevWorkshop.Web.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet("")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet("spa")]
        [Authorize]
        public IActionResult Spa()
        {
            return View();
        }
    }
}