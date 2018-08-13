using Microsoft.AspNetCore.Http;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace WebDevWorkshop.Api.Middlewares
{
    public class SpotifyProxyMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ISpotifyAuthenticationService _spotifyAuthenticationService;
        private readonly SpotifyProxyOptions _options;

        public SpotifyProxyMiddleware(RequestDelegate next, ISpotifyAuthenticationService spotifyAuthenticationService, SpotifyProxyOptions options)
        {
            this._next = next;
            this._spotifyAuthenticationService = spotifyAuthenticationService;
            this._options = options;
        }

        public async Task InvokeAsync(HttpContext ctx)
        {
            if (ctx.Request.Path == "/" || ctx.Request.Path == "/favicon.ico")
            {
                await _next(ctx);
                return;
            }

            string accessToken;
            try
            {
                accessToken = await _spotifyAuthenticationService.GetAccessToken();
            }
            catch
            {
                ctx.Response.StatusCode = 500;
                return;
            }
            
            var method = ctx.Request.Method == "GET" ? HttpMethod.Get : HttpMethod.Post;
            var message = new HttpRequestMessage(method, "https://api.spotify.com" + ctx.Request.Path + ctx.Request.QueryString.Value);
            message.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var client = new HttpClient();
            var resp = await client.SendAsync(message);

            ctx.Response.StatusCode = (int)resp.StatusCode;
            await resp.Content.CopyToAsync(ctx.Response.Body);
        }
    }
}
