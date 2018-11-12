// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace Hypomos.IdentityServer.Quickstart.Home
{
    using System.Threading.Tasks;

    using IdentityServer4.Services;

    using Microsoft.AspNetCore.Mvc;

    [SecurityHeaders]
    public class HomeController : Controller
    {
        private readonly IIdentityServerInteractionService _interaction;

        public HomeController(IIdentityServerInteractionService interaction)
        {
            this._interaction = interaction;
        }

        /// <summary>
        ///     Shows the error page
        /// </summary>
        public async Task<IActionResult> Error(string errorId)
        {
            var vm = new ErrorViewModel();

            // retrieve error details from identityserver
            var message = await this._interaction.GetErrorContextAsync(errorId);
            if (message != null) vm.Error = message;

            return this.View("Error", vm);
        }

        public IActionResult Index()
        {
            return this.View();
        }
    }
}