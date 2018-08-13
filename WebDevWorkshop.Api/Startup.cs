using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Threading.Tasks;
using WebDevWorkshop.Api.Middlewares;

namespace WebDevWorkshop.Api
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            Config = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appSettings.json", true)
                .AddEnvironmentVariables("WDW_")
                .Build();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options => {
                    options.Authority = Config["oidc.authority"];
                    options.RequireHttpsMetadata = Config.GetValue<bool>("oidc.requireHttpsMetaData");
                    options.Audience = "WDWApi";
                });

            services.AddSpotifyProxy(Config["SpotifyClientId"], Config["SpotifySecret"]);

            services.AddCors();
        }
        
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseCors(policy => {
                policy.WithOrigins(Config["cors.host"])
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
            });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

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

            app.UseSpotifyProxy();
        }

        private IConfiguration Config { get; }
    }
}
