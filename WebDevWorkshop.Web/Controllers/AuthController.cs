using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace WebDevWorkshop.Web.Controllers
{
    [Route("auth")]
    public class AuthController : Controller
    {
        [HttpGet("signin")]
        public IActionResult SignIn(string returnUrl = "/spa")
        {
            if (!Url.IsLocalUrl(returnUrl))
            {
                returnUrl = "/spa";
            }

            return Challenge(new AuthenticationProperties { RedirectUri = returnUrl }, OpenIdConnectDefaults.AuthenticationScheme);
        }

        [HttpPost("signout")]
        public async Task SignOut()
        {
            await HttpContext.SignOutAsync();
            await HttpContext.SignOutAsync(OpenIdConnectDefaults.AuthenticationScheme);
        }

        [HttpGet("silentsignincallback")]
        public IActionResult SilentSignInCallback() => View();
    }
}