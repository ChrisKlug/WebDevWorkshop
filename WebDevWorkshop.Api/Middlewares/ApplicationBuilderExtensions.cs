using System;
using WebDevWorkshop.Api.Middlewares;

namespace Microsoft.AspNetCore.Builder
{
    public static class ApplicationBuilderExtensions
    {
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
