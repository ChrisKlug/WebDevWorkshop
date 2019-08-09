using System.Net.Http;
using WebDevWorkshop.Api;
using WebDevWorkshop.Api.Services;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSpotifyProxy(this IServiceCollection services)
        {
            return services.AddSingleton<ISpotifyAuthenticationService, SpotifyAuthenticationService>();
        }


        public static IServiceCollection AddSpotifyProxy(this IServiceCollection services, string clientId, string secret)
        {
            var config = new SpotifyConfig
            {
                ClientId = clientId,
                Secret = secret
            };
            services.AddSingleton(Options.Options.Create(config));
            return services.AddSpotifyProxy();
        }
    }
}
