using Microsoft.Extensions.DependencyInjection;
using System;
using WebDevWorkshop.Api.Middlewares;

namespace Microsoft.AspNetCore.Builder
{
    public static class SpotifyProxyExtensions
    {
        public static IServiceCollection AddSpotifyProxy(this IServiceCollection services, string clientId, string clientSecret)
        {
            return services.AddSingleton<ISpotifyAuthenticationService>(new SpotifyAuthenticationService(clientId, clientSecret));
        }

        public static IApplicationBuilder UseSpotifyProxy(this IApplicationBuilder app, SpotifyProxyOptions options = null)
        {
            if (options == null)
                options = new SpotifyProxyOptions();

            return app.UseMiddleware<SpotifyProxyMiddleware>(options);
        }

        public static IApplicationBuilder UseSpotifyProxy(this IApplicationBuilder app, Action<SpotifyProxyOptions> configCallback)
        {
            var options = new SpotifyProxyOptions();
            configCallback(options);
            return app.UseMiddleware<SpotifyProxyMiddleware>(options);
        }
    }
}
