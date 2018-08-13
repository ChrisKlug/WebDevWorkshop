using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Newtonsoft.Json;
using System.IO;
using System.Threading.Tasks;

namespace WebDevWorkshop.Web
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            Config = new ConfigurationBuilder()
            .SetBasePath(env.ContentRootPath)
            .AddJsonFile("appSettings.json", false)
            .AddEnvironmentVariables("WDW_")
            .Build();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddOpenIdConnect(options => {
                    options.Authority = Config["oidc.authority"];
                    options.RequireHttpsMetadata = Config.GetValue<bool>("oidc.requireHttpsMetadata");
                    options.ClientId = "WDWWeb";
                    options.SignedOutRedirectUri = "/";
                    options.SaveTokens = true;
                    options.Events.OnRemoteFailure = (ctx) =>
                    {
                        ctx.Response.Redirect("/");
                        ctx.HandleResponse();
                        return Task.CompletedTask;
                    };
                    options.TokenValidationParameters.NameClaimType = "name";
                })
                .AddCookie(options => {
                    options.LoginPath = "/auth/signin";
                });

            services.AddMvc();
        }
        
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();

                app.UseStaticFiles(new StaticFileOptions {
                    RequestPath = "/node_modules",
                    FileProvider = new PhysicalFileProvider(Path.Combine(env.ContentRootPath, "node_modules"))
                });
                app.UseStaticFiles(new StaticFileOptions
                {
                    RequestPath = "/SPA",
                    FileProvider = new PhysicalFileProvider(Path.Combine(env.ContentRootPath, "SPA"))
                });
                app.UseStaticFiles(new StaticFileOptions
                {
                    RequestPath = "/build",
                    FileProvider = new PhysicalFileProvider(Path.Combine(env.ContentRootPath, "build"))
                });
            }
            
            app.UseWhen(ctx => ctx.Request.Path == "/spa-config.js", app2 =>
            {
                var spaConfig = JsonConvert.SerializeObject(new { oidcAuthority = Config["oidc.authority"], apiBaseUrl = Config["api.baseUrl"] });
                spaConfig = "window.spaConfig = " + spaConfig;
                app2.Run(async ctx =>
                {
                    await ctx.Response.WriteAsync(spaConfig);
                });
            });

            app.UseStaticFiles();

            app.UseAuthentication();

            app.UseMvc();
        }

        private IConfiguration Config { get; }
    }
}
