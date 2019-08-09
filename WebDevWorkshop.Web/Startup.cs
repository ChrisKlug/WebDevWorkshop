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
        private readonly IConfiguration _config;

        public Startup(IConfiguration config)
        {
            _config = config;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddOpenIdConnect(options => {
                    options.Authority = _config["oidc.authority"];
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
                    RequestPath = "/TypeScript",
                    FileProvider = new PhysicalFileProvider(Path.Combine(env.ContentRootPath, "TypeScript"))
                });
            }
            else
            {
                app.UseHsts(hsts => hsts.MaxAge(365));
            }

            app.UseHttpsRedirection();

            app.UseCsp(options =>
            {
                options.DefaultSources(x => x.Self())
                    .ConnectSources(x => x.CustomSources(_config["oidc.authority"], _config["api.endpoint"]))
                    .ScriptSources(x => x.Self().UnsafeInline().UnsafeEval())
                    .FrameSources(x => x.Self().CustomSources(_config["oidc.authority"]))
                    .ImageSources(x => x.Self().CustomSources("https://placekitten.com/", "https://i.scdn.co/"));
            });

            app.UseStaticFiles();

            app.UseWhen(ctx => ctx.Request.Path == "/spa-config.js", app2 =>
            {
                var spaConfig = JsonConvert.SerializeObject(new { oidcAuthority = _config["oidc.authority"], apiEndpoint = _config["api.endpoint"] });
                spaConfig = "window.spaConfig = " + spaConfig;
                app2.Run(async ctx =>
                {
                    await ctx.Response.WriteAsync(spaConfig);
                });
            });

            app.UseAuthentication();

            app.UseMvc();

            app.Run(async (context) =>
            {
                await context.Response.WriteAsync("Hello World!");
            });
        }
    }
}
