// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace Hypomos.IdentityServer.Quickstart.Account
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Claims;
    using System.Security.Principal;
    using System.Threading.Tasks;

    using IdentityModel;

    using IdentityServer4;
    using IdentityServer4.Events;
    using IdentityServer4.Extensions;
    using IdentityServer4.Models;
    using IdentityServer4.Services;
    using IdentityServer4.Stores;
    using IdentityServer4.Test;

    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;

    /// <summary>
    ///     This sample controller implements a typical login/logout/provision workflow for local and external accounts.
    ///     The login service encapsulates the interactions with the user data store. This data store is in-memory only and
    ///     cannot be used for production!
    ///     The interaction service provides a way for the UI to communicate with identityserver for validation and context
    ///     retrieval
    /// </summary>
    [SecurityHeaders]
    public class AccountController : Controller
    {
        private readonly IClientStore _clientStore;

        private readonly IEventService _events;

        private readonly IIdentityServerInteractionService _interaction;

        private readonly IAuthenticationSchemeProvider _schemeProvider;

        private readonly TestUserStore _users;

        public AccountController(
            IIdentityServerInteractionService interaction,
            IClientStore clientStore,
            IAuthenticationSchemeProvider schemeProvider,
            IEventService events,
            TestUserStore users = null)
        {
            // if the TestUserStore is not in DI, then we'll just use the global users collection
            // this is where you would plug in your own custom identity management library (e.g. ASP.NET Identity)
            this._users = users ?? new TestUserStore(TestUsers.Users);

            this._interaction = interaction;
            this._clientStore = clientStore;
            this._schemeProvider = schemeProvider;
            this._events = events;
        }

        /// <summary>
        ///     initiate roundtrip to external authentication provider
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> ExternalLogin(string provider, string returnUrl)
        {
            if (AccountOptions.WindowsAuthenticationSchemeName == provider)
                return await this.ProcessWindowsLoginAsync(returnUrl);

            // start challenge and roundtrip the return URL and 
            var props = new AuthenticationProperties
                            {
                                RedirectUri = this.Url.Action("ExternalLoginCallback"),
                                Items = {
                                           { "returnUrl", returnUrl }, { "scheme", provider } 
                                        }
                            };
            return this.Challenge(props, provider);
        }

        /// <summary>
        ///     Post processing of external authentication
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> ExternalLoginCallback()
        {
            // read external identity from the temporary cookie
            var result =
                await this.HttpContext.AuthenticateAsync(IdentityServerConstants.ExternalCookieAuthenticationScheme);
            if (result?.Succeeded != true) throw new Exception("External authentication error");

            // lookup our user and external provider info
            var (user, provider, providerUserId, claims) = this.FindUserFromExternalProvider(result);
            if (user == null) user = this.AutoProvisionUser(provider, providerUserId, claims);

            // this allows us to collect any additonal claims or properties
            // for the specific prtotocols used and store them in the local auth cookie.
            // this is typically used to store data needed for signout from those protocols.
            var additionalLocalClaims = new List<Claim>();
            var localSignInProps = new AuthenticationProperties();
            this.ProcessLoginCallbackForOidc(result, additionalLocalClaims, localSignInProps);
            this.ProcessLoginCallbackForWsFed(result, additionalLocalClaims, localSignInProps);
            this.ProcessLoginCallbackForSaml2p(result, additionalLocalClaims, localSignInProps);

            // issue authentication cookie for user
            await this._events.RaiseAsync(
                new UserLoginSuccessEvent(provider, providerUserId, user.SubjectId, user.Username));
            await this.HttpContext.SignInAsync(
                user.SubjectId,
                user.Username,
                provider,
                localSignInProps,
                additionalLocalClaims.ToArray());

            // delete temporary cookie used during external authentication
            await this.HttpContext.SignOutAsync(IdentityServerConstants.ExternalCookieAuthenticationScheme);

            // validate return URL and redirect back to authorization endpoint or a local page
            var returnUrl = result.Properties.Items["returnUrl"];
            if (this._interaction.IsValidReturnUrl(returnUrl) || this.Url.IsLocalUrl(returnUrl))
                return this.Redirect(returnUrl);

            return this.Redirect("~/");
        }

        /// <summary>
        ///     Show login page
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Login(string returnUrl)
        {
            // build a model so we know what to show on the login page
            var vm = await this.BuildLoginViewModelAsync(returnUrl);

            if (vm.IsExternalLoginOnly) return await this.ExternalLogin(vm.ExternalLoginScheme, returnUrl);

            return this.View(vm);
        }

        /// <summary>
        ///     Handle postback from username/password login
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginInputModel model, string button)
        {
            if (button != "login")
            {
                // the user clicked the "cancel" button
                var context = await this._interaction.GetAuthorizationContextAsync(model.ReturnUrl);
                if (context != null)
                {
                    // if the user cancels, send a result back into IdentityServer as if they 
                    // denied the consent (even if this client does not require consent).
                    // this will send back an access denied OIDC error response to the client.
                    await this._interaction.GrantConsentAsync(context, ConsentResponse.Denied);

                    // we can trust model.ReturnUrl since GetAuthorizationContextAsync returned non-null
                    return this.Redirect(model.ReturnUrl);
                }

                // since we don't have a valid context, then we just go back to the home page
                return this.Redirect("~/");
            }

            if (this.ModelState.IsValid)
            {
                // validate username/password against in-memory store
                if (this._users.ValidateCredentials(model.Username, model.Password))
                {
                    var user = this._users.FindByUsername(model.Username);
                    await this._events.RaiseAsync(
                        new UserLoginSuccessEvent(user.Username, user.SubjectId, user.Username));

                    // only set explicit expiration here if user chooses "remember me". 
                    // otherwise we rely upon expiration configured in cookie middleware.
                    AuthenticationProperties props = null;
                    if (AccountOptions.AllowRememberLogin && model.RememberLogin)
                        props = new AuthenticationProperties
                                    {
                                        IsPersistent = true,
                                        ExpiresUtc = DateTimeOffset.UtcNow.Add(AccountOptions.RememberMeLoginDuration)
                                    };
                    

                    // issue authentication cookie with subject ID and username
                    await this.HttpContext.SignInAsync(user.SubjectId, user.Username, props);

                    // make sure the returnUrl is still valid, and if so redirect back to authorize endpoint or a local page
                    // the IsLocalUrl check is only necessary if you want to support additional local pages, otherwise IsValidReturnUrl is more strict
                    if (this._interaction.IsValidReturnUrl(model.ReturnUrl) || this.Url.IsLocalUrl(model.ReturnUrl))
                        return this.Redirect(model.ReturnUrl);

                    return this.Redirect("~/");
                }

                await this._events.RaiseAsync(new UserLoginFailureEvent(model.Username, "invalid credentials"));

                this.ModelState.AddModelError(string.Empty, AccountOptions.InvalidCredentialsErrorMessage);
            }

            // something went wrong, show form with error
            var vm = await this.BuildLoginViewModelAsync(model);
            return this.View(vm);
        }

        /// <summary>
        ///     Show logout page
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Logout(string logoutId)
        {
            // build a model so the logout page knows what to display
            var vm = await this.BuildLogoutViewModelAsync(logoutId);

            if (vm.ShowLogoutPrompt == false) return await this.Logout(vm);

            return this.View(vm);
        }

        /// <summary>
        ///     Handle logout page postback
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout(LogoutInputModel model)
        {
            // build a model so the logged out page knows what to display
            var vm = await this.BuildLoggedOutViewModelAsync(model.LogoutId);

            if (this.User?.Identity.IsAuthenticated == true)
            {
                // delete local authentication cookie
                await this.HttpContext.SignOutAsync();

                // raise the logout event
                await this._events.RaiseAsync(
                    new UserLogoutSuccessEvent(this.User.GetSubjectId(), this.User.GetDisplayName()));
            }

            // check if we need to trigger sign-out at an upstream identity provider
            if (vm.TriggerExternalSignout)
            {
                // build a return URL so the upstream provider will redirect back
                // to us after the user has logged out. this allows us to then
                // complete our single sign-out processing.
                var url = this.Url.Action("Logout", new { logoutId = vm.LogoutId });

                // this triggers a redirect to the external provider for sign-out
                return this.SignOut(
                    new AuthenticationProperties { RedirectUri = url },
                    vm.ExternalAuthenticationScheme);
            }

            return this.View("LoggedOut", vm);
        }

        private TestUser AutoProvisionUser(string provider, string providerUserId, IEnumerable<Claim> claims)
        {
            var user = this._users.AutoProvisionUser(provider, providerUserId, claims.ToList());
            return user;
        }

        private async Task<LoggedOutViewModel> BuildLoggedOutViewModelAsync(string logoutId)
        {
            // get context information (client name, post logout redirect URI and iframe for federated signout)
            var logout = await this._interaction.GetLogoutContextAsync(logoutId);

            var vm = new LoggedOutViewModel
                         {
                             AutomaticRedirectAfterSignOut = AccountOptions.AutomaticRedirectAfterSignOut,
                             PostLogoutRedirectUri = logout?.PostLogoutRedirectUri,
                             ClientName =
                                 string.IsNullOrEmpty(logout?.ClientName) ? logout?.ClientId : logout?.ClientName,
                             SignOutIframeUrl = logout?.SignOutIFrameUrl,
                             LogoutId = logoutId
                         };

            if (this.User?.Identity.IsAuthenticated == true)
            {
                var idp = this.User.FindFirst(JwtClaimTypes.IdentityProvider)?.Value;
                if (idp != null && idp != IdentityServerConstants.LocalIdentityProvider)
                {
                    var providerSupportsSignout = await this.HttpContext.GetSchemeSupportsSignOutAsync(idp);
                    if (providerSupportsSignout)
                    {
                        if (vm.LogoutId == null) vm.LogoutId = await this._interaction.CreateLogoutContextAsync();

                        vm.ExternalAuthenticationScheme = idp;
                    }
                }
            }

            return vm;
        }

        /*****************************************/
        /* helper APIs for the AccountController */
        /*****************************************/
        private async Task<LoginViewModel> BuildLoginViewModelAsync(string returnUrl)
        {
            var context = await this._interaction.GetAuthorizationContextAsync(returnUrl);
            if (context?.IdP != null)
                return new LoginViewModel
                           {
                               EnableLocalLogin = false,
                               ReturnUrl = returnUrl,
                               Username = context?.LoginHint,
                               ExternalProviders = new[] { new ExternalProvider { AuthenticationScheme = context.IdP } }
                           };

            var schemes = await this._schemeProvider.GetAllSchemesAsync();

            var providers = schemes
                .Where(
                    x => x.DisplayName != null || x.Name.Equals(
                             AccountOptions.WindowsAuthenticationSchemeName,
                             StringComparison.OrdinalIgnoreCase)).Select(
                    x => new ExternalProvider { DisplayName = x.DisplayName, AuthenticationScheme = x.Name }).ToList();

            var allowLocal = true;
            if (context?.ClientId != null)
            {
                var client = await this._clientStore.FindEnabledClientByIdAsync(context.ClientId);
                if (client != null)
                {
                    allowLocal = client.EnableLocalLogin;

                    if (client.IdentityProviderRestrictions != null && client.IdentityProviderRestrictions.Any())
                        providers = providers.Where(
                                provider => client.IdentityProviderRestrictions.Contains(provider.AuthenticationScheme))
                            .ToList();
                }
            }

            return new LoginViewModel
                       {
                           AllowRememberLogin = AccountOptions.AllowRememberLogin,
                           EnableLocalLogin = allowLocal && AccountOptions.AllowLocalLogin,
                           ReturnUrl = returnUrl,
                           Username = context?.LoginHint,
                           ExternalProviders = providers.ToArray()
                       };
        }

        private async Task<LoginViewModel> BuildLoginViewModelAsync(LoginInputModel model)
        {
            var vm = await this.BuildLoginViewModelAsync(model.ReturnUrl);
            vm.Username = model.Username;
            vm.RememberLogin = model.RememberLogin;
            return vm;
        }

        private async Task<LogoutViewModel> BuildLogoutViewModelAsync(string logoutId)
        {
            var vm = new LogoutViewModel { LogoutId = logoutId, ShowLogoutPrompt = AccountOptions.ShowLogoutPrompt };

            if (this.User?.Identity.IsAuthenticated != true)
            {
                // if the user is not authenticated, then just show logged out page
                vm.ShowLogoutPrompt = false;
                return vm;
            }

            var context = await this._interaction.GetLogoutContextAsync(logoutId);
            if (context?.ShowSignoutPrompt == false)
            {
                // it's safe to automatically sign-out
                vm.ShowLogoutPrompt = false;
                return vm;
            }

            // show the logout prompt. this prevents attacks where the user
            // is automatically signed out by another malicious web page.
            return vm;
        }

        private (TestUser user, string provider, string providerUserId, IEnumerable<Claim> claims)
            FindUserFromExternalProvider(AuthenticateResult result)
        {
            var externalUser = result.Principal;

            // try to determine the unique id of the external user (issued by the provider)
            // the most common claim type for that are the sub claim and the NameIdentifier
            // depending on the external provider, some other claim type might be used
            var userIdClaim = externalUser.FindFirst(JwtClaimTypes.Subject)
                              ?? externalUser.FindFirst(ClaimTypes.NameIdentifier)
                              ?? throw new Exception("Unknown userid");

            // remove the user id claim so we don't include it as an extra claim if/when we provision the user
            var claims = externalUser.Claims.ToList();
            claims.Remove(userIdClaim);

            var provider = result.Properties.Items["scheme"];
            var providerUserId = userIdClaim.Value;

            // find external user
            var user = this._users.FindByExternalProvider(provider, providerUserId);

            return (user, provider, providerUserId, claims);
        }

        private void ProcessLoginCallbackForOidc(
            AuthenticateResult externalResult,
            List<Claim> localClaims,
            AuthenticationProperties localSignInProps)
        {
            // if the external system sent a session id claim, copy it over
            // so we can use it for single sign-out
            var sid = externalResult.Principal.Claims.FirstOrDefault(x => x.Type == JwtClaimTypes.SessionId);
            if (sid != null) localClaims.Add(new Claim(JwtClaimTypes.SessionId, sid.Value));

            // if the external provider issued an id_token, we'll keep it for signout
            var id_token = externalResult.Properties.GetTokenValue("id_token");
            if (id_token != null)
                localSignInProps.StoreTokens(new[] { new AuthenticationToken { Name = "id_token", Value = id_token } });
        }

        private void ProcessLoginCallbackForSaml2p(
            AuthenticateResult externalResult,
            List<Claim> localClaims,
            AuthenticationProperties localSignInProps)
        {
        }

        private void ProcessLoginCallbackForWsFed(
            AuthenticateResult externalResult,
            List<Claim> localClaims,
            AuthenticationProperties localSignInProps)
        {
        }

        private async Task<IActionResult> ProcessWindowsLoginAsync(string returnUrl)
        {
            // see if windows auth has already been requested and succeeded
            var result = await this.HttpContext.AuthenticateAsync(AccountOptions.WindowsAuthenticationSchemeName);
            if (result?.Principal is WindowsPrincipal wp)
            {
                // we will issue the external cookie and then redirect the
                // user back to the external callback, in essence, tresting windows
                // auth the same as any other external authentication mechanism
                var props = new AuthenticationProperties
                                {
                                    RedirectUri = this.Url.Action("ExternalLoginCallback"),
                                    Items =
                                        {
                                            { "returnUrl", returnUrl },
                                            { "scheme", AccountOptions.WindowsAuthenticationSchemeName }
                                        }
                                };

                var id = new ClaimsIdentity(AccountOptions.WindowsAuthenticationSchemeName);
                id.AddClaim(new Claim(JwtClaimTypes.Subject, wp.Identity.Name));
                id.AddClaim(new Claim(JwtClaimTypes.Name, wp.Identity.Name));

                // add the groups as claims -- be careful if the number of groups is too large
                if (AccountOptions.IncludeWindowsGroups)
                {
                    var wi = wp.Identity as WindowsIdentity;
                    var groups = wi.Groups.Translate(typeof(NTAccount));
                    var roles = groups.Select(x => new Claim(JwtClaimTypes.Role, x.Value));
                    id.AddClaims(roles);
                }

                await this.HttpContext.SignInAsync(
                    IdentityServerConstants.ExternalCookieAuthenticationScheme,
                    new ClaimsPrincipal(id),
                    props);
                return this.Redirect(props.RedirectUri);
            }

            // trigger windows auth
            // since windows auth don't support the redirect uri,
            // this URL is re-triggered when we call challenge
            return this.Challenge(AccountOptions.WindowsAuthenticationSchemeName);
        }
    }
}