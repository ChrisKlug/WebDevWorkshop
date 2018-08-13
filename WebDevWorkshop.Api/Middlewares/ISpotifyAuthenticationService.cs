using System.Threading.Tasks;

namespace WebDevWorkshop.Api.Middlewares
{
    public interface ISpotifyAuthenticationService
    {
        Task<string> GetAccessToken();
    }
}
