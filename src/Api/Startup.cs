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
            var authority = this.Configuration["Authority"];
            var origin = this.Configuration["Origin"];

            services.AddMvcCore()
                    .AddAuthorization()
                    .AddJsonFormatters()
                    .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddAuthentication("Bearer")
                    .AddJwtBearer(options =>
                    {
                        options.Authority = authority;
                        options.RequireHttpsMetadata = false;

                        options.Audience = "api1";
                    });

            services.AddCors(options =>
            {
                // this defines a CORS policy called "default"
                options.AddPolicy("default",
                                  policy =>
                                  {
                                      policy.WithOrigins(origin)
                                            .AllowAnyHeader()
                                            .AllowAnyMethod();
                                  });
            });
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            var allowedHosts = this.Configuration["AllowedHosts"];

            app.UseReverseProxy(new ReverseProxyOptions
            {
                AllowedHosts = new List<string>
                {
                    allowedHosts
                }
            });

            app.UseCors("default");
            app.UseAuthentication();
            app.UseMvc();
        }
    }
}