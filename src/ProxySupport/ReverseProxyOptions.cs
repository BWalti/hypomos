namespace ProxySupport
{
    using Microsoft.AspNetCore.Builder;

    public class ReverseProxyOptions : ForwardedHeadersOptions
    {
        /// <summary>
        /// Gets or sets a path prefix, which the proxy removed.
        /// The proxy will forward a client request like '/serviceA/Controller/Action' e.g. as '/Controller/Action'.
        /// Thus this Request Pipeline needs to set the PathBase without knowledge of it to fix absolute navigation links etc.
        /// </summary>
        public string ProxyHidesPathPrefix { get; set; } = "";

        /// <summary>
        /// Gets or sets a synthetically added path prefix by the proxy,
        /// delivered to this service, which needs to be removed before processing the Request.
        ///
        /// Meaning a call to a proxy like '/serviceA/Controller/Action' and setting this value to '/serviceA'
        /// results in a request path as '/Controller/Action' and the PathBase gets set to '/serviceA'
        /// (resulting in fixed absolute navigation links, etc.)
        /// </summary>
        public string ProxySynthPathPrefix { get; set; } = "";
    }
}