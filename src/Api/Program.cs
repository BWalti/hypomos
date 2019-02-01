namespace Hypomos.Api
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
            Console.Title = "API";

            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";
            //var isDevelopment = "Development".Equals(environment, StringComparison.OrdinalIgnoreCase);

            var config = new ConfigurationBuilder()
                         .SetBasePath(Directory.GetCurrentDirectory())
                         .AddCommandLine(args)
                         .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                         .AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: true)
                         .AddUserSecrets<Program>(optional: true)
                         .Build();

            WebHost.CreateDefaultBuilder(args)
                   .UseConfiguration(config)
                   .UseStartup<Startup>()
                   .Build()
                   .Run();
        }
    }
}