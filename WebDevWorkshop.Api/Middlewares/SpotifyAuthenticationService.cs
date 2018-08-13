using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace WebDevWorkshop.Api.Middlewares
{
    public class SpotifyAuthenticationService : ISpotifyAuthenticationService
    {

        private readonly string _clientId;
        private readonly string _clientSecret;
        private Task<string> _accessTokenRetrievalTask;
        private DateTime _tokenExpiry = DateTime.MinValue;
        private object _lockObject = new Object();

        public SpotifyAuthenticationService(string clientId, string clientSecret)
        {
            _clientId = clientId;
            _clientSecret = clientSecret;
        }

        public Task<string> GetAccessToken()
        {
            if (_accessTokenRetrievalTask == null || _tokenExpiry < DateTime.Now)
            {
                lock (_lockObject)
                {
                    _accessTokenRetrievalTask = SignIn();
                }
            }

            return _accessTokenRetrievalTask;
        }

        private Task<string> SignIn()
        {
            return Task.Run(async () =>
            {
                var client = new HttpClient();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.ASCII.GetBytes($"{_clientId}:{_clientSecret}")));
                var response = await client.PostAsync("https://accounts.spotify.com/api/token", new FormUrlEncodedContent(new[] { new KeyValuePair<string, string>("grant_type", "client_credentials") }));
                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception("Could not retrieve an access token");
                }

                var content = await response.Content.ReadAsStringAsync();
                var token = JObject.Parse(content);
                var expiry = token.Value<int>("expires_in");
                lock (_lockObject)
                {
                    _tokenExpiry = DateTime.Now.AddSeconds(expiry - 300);
                }

                return token.Value<string>("access_token");
            });
        }
    }
}
