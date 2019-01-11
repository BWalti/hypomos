namespace ProxySupport
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.HttpOverrides;

    public class ReverseProxyMiddleware
    {
        private readonly RequestDelegate next;
        private readonly ReverseProxyOptions options;

        public ReverseProxyMiddleware(RequestDelegate next, ReverseProxyOptions options)
        {
            this.next = next;
            this.options = options;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            httpContext.Request.Scheme = httpContext.Request.Headers[ForwardedHeadersDefaults.XOriginalProtoHeaderName];
            httpContext.Request.Host = new HostString(httpContext.Request.Headers[ForwardedHeadersDefaults.XForwardedHostHeaderName]);

            httpContext.Request.PathBase = new PathString(this.options.ProxyHidesPathPrefix);
            
            await this.next(httpContext);
        }
    }
}