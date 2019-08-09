using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Threading.Tasks;

namespace WebDevWorkshop.Api
{
    public class Startup
    {
        public Startup(IConfiguration config)
        {
            Config = config;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHttpClient();

            services.Configure<SpotifyConfig>(Config.GetSection("Spotify"));

            services.AddSpotifyProxy();

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options => {
                    options.Authority = Config["oidc.authority"];
                    options.Audience = "WDWApi";
                });

            services.AddCors(options => {
                options.AddPolicy("Default", policy => {
                    policy.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials();
                });
            });
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILogger<Startup> logger)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseCors("Default");

            app.UseAuthentication();

            app.Use((ctx, next) =>
            {
                if (!ctx.User.Identity.IsAuthenticated)
                {
                    ctx.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    return Task.CompletedTask;
                }
                return next();
            });

            app.UseSpotifyProxy(options => {
                options.OnError = err => logger.LogError(err, "Error proxying call to Spotify");
            });
        }

        public IConfiguration Config { get; }
    }
}
