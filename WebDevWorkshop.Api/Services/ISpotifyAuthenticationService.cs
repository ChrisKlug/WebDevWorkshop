using System.Threading.Tasks;

namespace WebDevWorkshop.Api.Services
{
    public interface ISpotifyAuthenticationService
    {
        Task<string> GetAccessToken();
    }
}