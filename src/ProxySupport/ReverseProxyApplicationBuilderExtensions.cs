namespace ProxySupport
{
    using Microsoft.AspNetCore.Builder;

    public static class ReverseProxyApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseReverseProxy(this IApplicationBuilder app, ReverseProxyOptions options)
        {
            app.UseForwardedHeaders(options);
            app.UsePathBase(options.ProxySynthPathPrefix);
            app.UseMiddleware<ReverseProxyMiddleware>(options);

            return app;
        }
    }
}
