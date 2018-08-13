using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Quickstart.UI;
using IdentityServer4.Test;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Security.Claims;

namespace WebDevWorkshop.IdentityServer
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            Config = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appSettings.json")
                .AddEnvironmentVariables("WDW_")
                .Build();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddIdentityServer()
                .AddInMemoryClients(new Client[] {
                    new Client
                        {
                            ClientId = "WDWWeb",
                            AllowedGrantTypes = GrantTypes.Implicit,
                            AllowedScopes = { IdentityServerConstants.StandardScopes.OpenId, IdentityServerConstants.StandardScopes.Profile },
                            PostLogoutRedirectUris = { Config["clientHost"] + "/signout-callback-oidc" },
                            RedirectUris = { Config["clientHost"] + "/signin-oidc" },
                            RequireConsent = false,
                            AlwaysIncludeUserClaimsInIdToken = true
                        },
                    new Client
                        {
                            ClientId = "WDWSPA",
                            AllowedGrantTypes = GrantTypes.Implicit,
                            AllowedScopes = { IdentityServerConstants.StandardScopes.OpenId, IdentityServerConstants.StandardScopes.Profile, "WDWApi" },
                            RedirectUris = { Config["clientHost"] + "/auth/silentsignincallback" },
                            RequireConsent = false,
                            AlwaysIncludeUserClaimsInIdToken = true,
                            AllowAccessTokensViaBrowser = true,
                            AllowedCorsOrigins = { Config["clientHost"] }
                        }
                    })
                .AddInMemoryIdentityResources(new IdentityResource[]
                    {
                        new IdentityResources.OpenId(),
                        new IdentityResources.Profile()
                    })
                .AddInMemoryApiResources(new ApiResource[] {
                    new ApiResource("WDWApi")
                })
                .AddTestUsers(new List<TestUser>
                {
                    new TestUser
                    {
                        SubjectId = "1",
                        Username = "ZeroKoll",
                        Password = "test",
                        Claims =
                        {
                            new Claim("name", "Chris Klug")
                        }
                    }
                })
                .AddDeveloperSigningCredential();

            services.AddMvc();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts(hsts => hsts.MaxAge(365));
            }

            app.UseStaticFiles();

            AccountOptions.AutomaticRedirectAfterSignOut = true;
            AccountOptions.ShowLogoutPrompt = false;
            AccountOptions.AllowRememberLogin = false;

            app.UseIdentityServer();

            app.UseMvcWithDefaultRoute();
        }

        private IConfiguration Config { get; }
    }
}
