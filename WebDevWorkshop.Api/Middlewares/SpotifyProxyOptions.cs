using System;

namespace WebDevWorkshop.Api.Middlewares
{
    public class SpotifyProxyOptions
    {
        public Action<Exception> OnError { get; set; } = x => { };
    }
}
