using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace WebDevWorkshop.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((ctx, config) =>
                {
                    config
                        //.AddJsonFile("appSettings.json", false)
                        //.AddJsonFile($"appSettings.{ctx.HostingEnvironment.EnvironmentName}.json", true)
                        .AddEnvironmentVariables("WDW_");
                })
                .UseStartup<Startup>();
    }
}
