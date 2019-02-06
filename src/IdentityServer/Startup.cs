// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace Hypomos.IdentityServer
{
    using System.Collections.Generic;
    using IdentityServer4;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.HttpOverrides;
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

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            var allowedHosts = this.Configuration["AllowedHosts"];

            app.UseReverseProxy(new ReverseProxyOptions
            {
                ProxyHidesPathPrefix = "/auth",
                AllowedHosts = new List<string>
                {
                    allowedHosts
                },
                ForwardedHeaders = ForwardedHeaders.All
            });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseIdentityServer();

            app.UseStaticFiles();
            app.UseMvcWithDefaultRoute();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            var origin = this.Configuration["Origin"];

            services.AddMvc();

            // configure identity server with in-memory stores, keys, clients and scopes
            services
                .AddIdentityServer(options =>
                {
                    options.IssuerUri = $"{origin}/auth";
                    options.PublicOrigin = origin;
                })
                .AddDeveloperSigningCredential()
                .AddInMemoryIdentityResources(Config.GetIdentityResources())
                .AddInMemoryApiResources(Config.GetApiResources()).AddInMemoryClients(Config.GetClients(origin))
                .AddTestUsers(Config.GetUsers());

            services.AddAuthentication()
                .AddMicrosoftAccount(
                    options =>
                    {
                        options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;
                        options.SaveTokens = true;
                        options.ClientId = "55072778-bd0a-45f2-be06-3f5c0ad3e7c3";
                        options.ClientSecret = "qepoAWG35#!pdcRJBK935|-";
                    });
        }
    }
}