namespace Hypomos.Api
{
    using System.Collections.Generic;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using ProxySupport;

    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            this.Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvcCore()
                    .AddAuthorization()
                    .AddJsonFormatters()
                    .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddAuthentication("Bearer")
                    .AddJwtBearer(options =>
                    {
                        options.Authority = "http://localhost:5005";
                        options.RequireHttpsMetadata = false;

                        options.Audience = "api1";
                    });

            services.AddCors(options =>
            {
                // this defines a CORS policy called "default"
                options.AddPolicy("default",
                                  policy =>
                                  {
                                      policy.WithOrigins("http://localhost:5003")
                                            .AllowAnyHeader()
                                            .AllowAnyMethod();
                                  });
            });
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseReverseProxy(new ReverseProxyOptions
            {
                AllowedHosts = new List<string>
                {
                    "localhost:5005"
                }
            });

            app.UseCors("default");
            app.UseAuthentication();
            app.UseMvc();
        }
    }
}