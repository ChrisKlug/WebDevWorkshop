using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace WebDevWorkshop.Web.Controllers
{
    [Route("auth")]
    public class AuthController : Controller
    {
        [Route("signin")]
        public IActionResult SignIn(string returnUrl = "/spa") => 
            Challenge(new AuthenticationProperties { RedirectUri = returnUrl }, OpenIdConnectDefaults.AuthenticationScheme);

        [Route("signout")]
        [HttpPost]
        public async Task SignOut()
        {
            await HttpContext.SignOutAsync();
            await HttpContext.SignOutAsync(OpenIdConnectDefaults.AuthenticationScheme);
        }

        [Route("silentsignincallback")]
        public IActionResult SilentSignInCallback()
        {
            return View();
        }
    }
}