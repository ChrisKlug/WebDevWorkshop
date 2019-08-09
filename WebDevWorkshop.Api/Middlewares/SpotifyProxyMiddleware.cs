using Microsoft.AspNetCore.Http;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using WebDevWorkshop.Api.Services;

namespace WebDevWorkshop.Api.Middlewares
{
    public class SpotifyProxyMiddleware
    {

        private readonly RequestDelegate _next;
        private readonly SpotifyProxyOptions _options;
        private readonly ISpotifyAuthenticationService _spotifyAuthenticationService;
        private readonly IHttpClientFactory _httpClientFactory;

        public SpotifyProxyMiddleware(RequestDelegate next, SpotifyProxyOptions options, ISpotifyAuthenticationService spotifyAuthenticationService, IHttpClientFactory httpClientFactory)
        {
            _next = next;
            _options = options;
            _spotifyAuthenticationService = spotifyAuthenticationService;
            _httpClientFactory = httpClientFactory;
        }

        public async Task InvokeAsync(HttpContext ctx)
        {
            if (ctx.Request.Path == "/" || ctx.Request.Path == "/favicon")
            {
                await _next(ctx);
                return;
            }

            string accessToken;
            try
            {
                accessToken = await _spotifyAuthenticationService.GetAccessToken();
            }
            catch (Exception ex)
            {
                _options.OnError(ex);
                ctx.Response.StatusCode = 500;
                return;
            }

            var method = ctx.Request.Method == "GET" ? HttpMethod.Get : HttpMethod.Post;
            var message = new HttpRequestMessage(method, "https://api.spotify.com" + ctx.Request.Path + ctx.Request.QueryString.Value);
            message.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var client = _httpClientFactory.CreateClient();
            var resp = await client.SendAsync(message);

            await Task.Delay(3000);

            ctx.Response.StatusCode = (int)resp.StatusCode;
            await resp.Content.CopyToAsync(ctx.Response.Body);
        }
    }
}
