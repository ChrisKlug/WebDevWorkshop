using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebDevWorkshop.Web.Controllers
{
    public class HomeController : Controller
    {
        [Route("")]
        public IActionResult Index()
        {
            return View();
        }

        [Route("spa")]
        [Authorize]
        public IActionResult Spa()
        {
            return View();
        }
    }
}