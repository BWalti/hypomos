namespace Gateway
{
    using System.IO;

    using Microsoft.AspNetCore;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;

    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args)
                .Build()
                .Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            return WebHost.CreateDefaultBuilder(args)
                          .UseContentRoot(Directory.GetCurrentDirectory())
                          .ConfigureAppConfiguration((hostingContext, config) =>
                          {
                              config
                                  .SetBasePath(hostingContext.HostingEnvironment.ContentRootPath)
                                  .AddJsonFile("appsettings.json", true, true)
                                  .AddJsonFile($"appsettings.{hostingContext.HostingEnvironment.EnvironmentName}.json", true, true)
                                  .AddJsonFile("ocelot.json")
                                  .AddEnvironmentVariables();
                          })
                          .UseStartup<Startup>();
        }
    }
}