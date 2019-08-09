using System;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace WebDevWorkshop.IdentityServer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseKestrel(options =>
                {
                    var environment = options.ApplicationServices.GetRequiredService<IHostingEnvironment>();
                    if (environment.IsProduction())
                    {
                        options.Listen(IPAddress.Any, 80);
                    } else
                    {
                        options.Listen(IPAddress.Loopback, 5000, listenOptions =>
                        {
                            listenOptions.UseHttps(LoadCertificate(StoreName.My, StoreLocation.LocalMachine, "localhost"));
                        });
                    }
                })
                .UseStartup<Startup>()
                .Build();

        private static X509Certificate2 LoadCertificate(StoreName storeName, StoreLocation location, string host)
        {
            using (var store = new X509Store(storeName, location))
            {
                store.Open(OpenFlags.ReadOnly);
                var certificate = store.Certificates.Find(
                    X509FindType.FindBySubjectName,
                    host, 
                    false);

                if (certificate.Count == 0)
                {
                    throw new InvalidOperationException($"Certificate not found for {host}.");
                }

                return certificate[0];
            }
        }
    }
}
