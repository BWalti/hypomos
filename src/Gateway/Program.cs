namespace Gateway
{
    using System;
    using System.IO;

    using Microsoft.AspNetCore;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;

    public class Program
    {
        public static void Main(string[] args)
        {
            WebHost.CreateDefaultBuilder(args)
                   .UseContentRoot(Directory.GetCurrentDirectory())
                   .ConfigureAppConfiguration((hostingContext, config) =>
                   {
                       config
                           .SetBasePath(hostingContext.HostingEnvironment.ContentRootPath)
                           .AddCommandLine(args)
                           .AddJsonFile("appsettings.json", true, true)
                           .AddJsonFile($"appsettings.{hostingContext.HostingEnvironment.EnvironmentName}.json", true, true)
                           .AddJsonFile("ocelot.json")
                           .AddJsonFile($"ocelot.{hostingContext.HostingEnvironment.EnvironmentName}.json", true, true)
                           .AddDockerSecrets("/run/secrets", optional: true)
                           .AddUserSecrets<Program>(optional: true)
                           .AddEnvironmentVariables();
                   })
                   .UseStartup<Startup>()
                .Build()
                .Run();
        }
    }
}